using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    /// <summary>
    /// Base for all Attack Actions
    /// </summary>
    public abstract class AttackBase : UnitTargetTileAction
    {

        public static readonly IReadOnlyList<MovementType> TARGETABLE_LAND_MOVEMENT_TYPES = new List<MovementType>()
        {
            MovementTypes.Boots.Instance,
            MovementTypes.Treads.Instance,
            MovementTypes.Wheels.Instance,
            MovementTypes.HalfTrack.Instance
        };

        public static readonly IReadOnlyList<MovementType> TARGETABLE_LAND_VEHICLE_MOVEMENT_TYPES = new List<MovementType>()
        {
            MovementTypes.Treads.Instance,
            MovementTypes.Wheels.Instance,
            MovementTypes.HalfTrack.Instance
        };

        public static readonly IReadOnlyList<MovementType> TARGETABLE_AIR_VEHICLE_MOVEMENT_TYPES = new List<MovementType>()
        {
            MovementTypes.Propeller.Instance
        };

        public abstract IReadOnlyList<MovementType> TargetableMovementTypes { get; }
        public virtual int ActionsNeeded { get; } = 1;
        public abstract int BaseAccuracy { get; }
        public virtual int MinimumRange { get; } = 1;
        public abstract int MaximumRange { get; }
        public abstract double ArmorPenetration { get; }
        public abstract double DamagePerSubunit { get; }
        public virtual double TerrainDamagePerSubunit { get; } = 0;
        public bool CanDamageTerrain { get { return TerrainDamagePerSubunit > 0; } }
        public abstract Dictionary<SupplyType, int> SuppliesNeeded { get; }

        public virtual int MaxSubunits { get; } = 5;

        public override bool CanRetaliate { get { return MinimumRange == 1 && MaximumRange == 1; } }

        public sealed override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser | ActionTriggers.AttackRetaliation;

        protected AttackBase(string key) : base("attack_" + key) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var unit = sourceTile.Unit;
            var targetUnit = targetTile.Unit;

            if (unit == null || (targetUnit == null && (CanDamageTerrain == false || targetTile.Terrain.Health <= 0)))
                return false;

            if (HasRequiredResources(state, context, sourceTile) == false)
                return false;

            if (CanTargetMovementType(targetTile.Unit.UnitType.MovementType) == false)
                return false;

            if (state.LocationsAroundPoint(sourceTile.Location, MinimumRange, MaximumRange).Contains(targetTile.Location) == false)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var changes = new List<StateChange>();

            var unit = sourceTile.Unit;
            var targetUnit = targetTile.Unit;

            #region Supply Cost

            var updatedSourceSupplies = unit.Supplies.ToDictionary(kp => kp.Key, kp => kp.Value);

            foreach (var kp in SuppliesNeeded)
            {
                updatedSourceSupplies[kp.Key] -= kp.Value;
            }
            
            changes.Add(new StateChanges.UnitStateChange(unit.UnitID, new Dictionary<string, object>()
            {
                { "Supplies", updatedSourceSupplies  },
                { "Actions", (context.Trigger == ActionTriggers.AttackRetaliation ? unit.Actions : unit.Actions - ActionsNeeded ) }
            }, sourceTile.Location));

            #endregion

            #region Damage to Target Unit

            bool enemyUnitDown = false;
            if(targetTile.Unit != null)
            {
                double damageCaused = CalculateDamage(
                    state,
                    context,
                    sourceTile,
                    targetTile,
                    GetAccuracy(state, context, sourceTile, targetTile),
                    GetUnitConcealment(state, context, sourceTile, targetTile),
                    GetDamage(state, context, sourceTile, targetTile),
                    GetUnitArmor(state, context, sourceTile, targetTile)
                );

                int roundedDamageCaused = RoundDamage(damageCaused);

                int newHealth = targetUnit.Health - roundedDamageCaused;
                enemyUnitDown = newHealth <= 0;

                if (roundedDamageCaused > 0)
                {
                        changes.Add(new StateChanges.UnitStateChange(targetUnit.UnitID, new Dictionary<string, object>()
                        {
                            { "Health", newHealth }
                        }, targetTile.Location, (enemyUnitDown ? StateChanges.UnitStateChange.Cause.Destroyed : StateChanges.UnitStateChange.Cause.Changed)));

                }
            }

            #endregion

            #region Damage to Target Terrain

            if (CanDamageTerrain && targetTile.Terrain.Health > 0)
            {
                double damageCaused = CalculateDamage(
                    state,
                    context,
                    sourceTile,
                    targetTile,
                    GetAccuracy(state, context, sourceTile, targetTile),
                    GetTerrainConcealment(state, context, sourceTile, targetTile),
                    GetTerrainDamage(state, context, sourceTile, targetTile),
                    GetTerrainDefense(state, context, sourceTile, targetTile)
                );
                int roundedDamageCaused = RoundDamage(damageCaused);
                int newHealth = targetTile.Terrain.Health - roundedDamageCaused;

                newHealth = Math.Max(0, newHealth);

                if(roundedDamageCaused > 0)
                {
                    changes.Add(new StateChanges.TerrainStateChange(targetTile.Location, new Dictionary<string, object>()
                    {
                        { "Health", newHealth }
                    }));
                }
            }

            #endregion

            // Prevent infinite loops
            if (enemyUnitDown == false && context.Trigger != ActionTriggers.AttackRetaliation)
                changes.AddRange(HandleRetaliation(state, changes, sourceTile, targetTile));


            return changes;
        }

        protected virtual double CalculateDamage(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile, int accuracy, int concealment, double attack, double defense)
        {
            double damageCaused = 0;

            var actualAccuracy = GetActualAccuracy(state, context, sourceTile, targetTile, accuracy, concealment);

            if (actualAccuracy > 0 && ArmorPenetration > defense)
            {
                damageCaused = (attack * (actualAccuracy / 100.0));
                    
                if(IsArmorAnImpedance(state, context, sourceTile, targetTile, defense))
                    damageCaused = damageCaused * GetDamagePenetration(state, context, sourceTile, targetTile, defense);

                if (damageCaused < 0)
                    damageCaused = 0;
            }

            return damageCaused;
        }

        protected virtual int RoundDamage(double damage)
        {
            return (int)Math.Round(damage, 0, MidpointRounding.AwayFromZero);
        }

        protected virtual bool IsArmorAnImpedance(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile, double defense)
        {
            return ArmorPenetration / 3.0 <= defense;
        }

        protected virtual double GetActualAccuracy(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile, int accuracy, int concealment)
        {
            var actualAccuracy = accuracy - concealment;
            if (actualAccuracy < 0)
                actualAccuracy = 0;

            return actualAccuracy;
        }

        protected virtual double GetDamagePenetration(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile, double defense)
        {
            return ((ArmorPenetration - (defense)) / ArmorPenetration);
        }

        protected virtual double GetDamage(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return DamagePerSubunit * GetSubunits(state, context, sourceTile, targetTile);
        }

        protected virtual double GetTerrainDamage(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return TerrainDamagePerSubunit * GetSubunits(state, context, sourceTile, targetTile);
        }

        protected virtual double GetSubunits(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return Math.Ceiling(sourceTile.Unit.Health / (sourceTile.Unit.UnitType.MaxHealth / (double)MaxSubunits));
        }

        protected virtual double GetUnitArmor(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return targetTile.Unit.Armor;
        }

        protected virtual double GetTerrainDefense(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return targetTile.Terrain.TerrainType.Toughness;
        }

        protected virtual int GetAccuracy(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var accuracy = BaseAccuracy;

            if (sourceTile.Unit.UnitType.EffectedByTerrainModifiers)
                accuracy += sourceTile.Terrain.TerrainType.AccuracyModifier;

            return accuracy;
        }

        protected virtual int GetUnitConcealment(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var concealment = 0;
            if (targetTile.Unit.UnitType.EffectedByTerrainModifiers)
            {
                var baseConcealment = targetTile.Terrain.TerrainType.ConcealmentModifier + targetTile.Unit.UnitType.Concealment;
                int digInBonus = ((int)Math.Round(baseConcealment * (targetTile.Terrain.DigIn * 2.0 / 5.0)));
                concealment = baseConcealment + digInBonus;
            }
            else
            {
                concealment = targetTile.Unit.UnitType.Concealment;
            }
            int height_modifier = GetUnitConcealmentHeightDifferenceModifier(state, context, sourceTile, targetTile);
            return concealment + height_modifier;
        }

        protected virtual int GetUnitConcealmentBase(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var concealment = targetTile.Unit.UnitType.Concealment;

            if (targetTile.Unit.UnitType.EffectedByTerrainModifiers)
                concealment += targetTile.Terrain.TerrainType.ConcealmentModifier;

            return concealment;
        }

        protected virtual int GetUnitConcealmentDigInBonus(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            if (targetTile.Unit.UnitType.EffectedByTerrainModifiers)
                return ((int)Math.Round(GetUnitConcealmentBase(state, context, sourceTile, targetTile) * (targetTile.Terrain.DigIn * 2.0 / 5.0)));
            else
                return 0;
        }

        protected virtual int GetUnitConcealmentHeightDifferenceModifier(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            int source_bonus = 0, target_bonus = 0;

            if (sourceTile.Unit.UnitType.EffectedByTerrainModifiers)
                source_bonus = -GetTerrainHeightModifier(sourceTile.Terrain.TerrainType.Height);

            if (targetTile.Unit.UnitType.EffectedByTerrainModifiers)
                target_bonus = GetTerrainHeightModifier(targetTile.Terrain.TerrainType.Height);

            return source_bonus + target_bonus;

        }

        protected virtual int GetTerrainHeightModifier(TerrainType.TerrainHeight height)
        {
            switch(height)
            {
                case TerrainType.TerrainHeight.Depressed:
                    return 0;
                case TerrainType.TerrainHeight.Normal:
                    return 5;
                case TerrainType.TerrainHeight.Elevated:
                    return 10;
                default:
                    throw new ArgumentException($"Unknown height {height}");
            }
        }

        protected virtual int GetTerrainConcealment(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            return 0;
        }

        protected virtual IReadOnlyList<StateChange> HandleRetaliation(IReadOnlyBattleGameState state, IReadOnlyList<StateChange> actionChanges, Tile sourceTile, Tile targetTile)
        {
            var forkedState = state.Fork();
            foreach (var change in actionChanges)
                forkedState.Update(change);

            var context = new ActionContext(null, ActionTriggers.AttackRetaliation, new UnitContext(targetTile.Location), new UnitContext(sourceTile.Location));

            foreach(var action in targetTile.Unit.UnitType.Actions.Where(a => a.ActionCategory == Category.Unit && a.ActionTargetCategory == TargetCategory.Tile && a.CanRetaliate == true))
            {
                if(action.CanPerformOn(forkedState, context))
                {
                    return action.PerformOn(forkedState, context);
                }
            }

            return new List<StateChange>(0);
        }

        /// <summary>
        /// Checks if Unit has enough resources to carry out an attack.
        /// </summary>
        /// <param name="state">Game State</param>
        /// <param name="sourceTile">Attacking Unit</param>
        /// <returns>True if the unit has the needed resources, false otherwise.</returns>
        protected bool HasRequiredResources(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != sourceTile);

            var unit = sourceTile.Unit;

            if (unit == null)
                return false;

            if (unit.Actions < ActionsNeeded && context.Trigger != ActionTriggers.AttackRetaliation)
                return false;

            foreach (var supply in SuppliesNeeded)
            {
                int unitSupply;

                if (unit.Supplies.TryGetValue(supply.Key, out unitSupply) == false)
                {
#if DEBUG
                    throw new ArgumentException(string.Format("Action {0} expects unit to have supplies of {1}, unit type of {2} does not, this unit can never perform this action.", this, supply.Key, unit.UnitType));
#else
                    return false;
#endif
                }

                if (unitSupply < supply.Value)
                    return false;

            }

            return true;
        }

        /// <summary>
        /// Checks if this type of attack can target the given movement type.
        /// </summary>
        /// <param name="type">Movement type in question</param>
        /// <returns>True if the unit can attack units of the given movement type, false otherwise.</returns>
        protected bool CanTargetMovementType(MovementType type)
        {
            return TargetableMovementTypes.Contains(type);
        }

        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            var dic = new Dictionary<Location, ActionChain>();

            if (sourceTile.Unit == null || HasRequiredResources(state, context, sourceTile) == false)
                return dic;

            // Get a list of possible tiles we could attack from (via movement).
            var sourceLocations = sourceTile.Unit.GetAvailableMovement(state);
            // Add the current tile we're on (it isn't returned above because we can't move to the tile we're on).
            // We pass null in as the action because we don't need to move.
            sourceLocations.Add(sourceTile.Location, null);

            foreach (var kp in  sourceLocations)
            {
                var sourceLocation = kp.Key;
                var action = kp.Value;

                var currentTile = state.GetTile(sourceLocation);
                if (currentTile == null)
                    continue;

                // Check every tile within range of our attack from the location.
                foreach (var location in state.LocationsAroundPoint(sourceLocation, MinimumRange, MaximumRange))
                {
                    ActionChain chain;
                    // If we've already found we can attack this location,
                    // we make sure we can't possibility attack it without moving,
                    // and if not (or we already have loaded that),
                    // we ignore it.
                    if (dic.TryGetValue(location, out chain) && (chain.Length == 1 || null != action))
                        continue;

                    var targetTile = state.GetTile(location);

                    // Check that there is a unit on the location we're targeting (and it isn't ourself)
                    if (targetTile == null || targetTile.Unit == null || location == sourceTile.Location)
                        continue;

                    // Check that we can attack the unit on this location.
                    if (targetTile.Unit.CommanderID != sourceTile.Unit.CommanderID && CanTargetMovementType(targetTile.Unit.UnitType.MovementType))
                    {
                        // Create the action chain and register it to our final list.
                        chain = new ActionChain();

                        if (action != null)
                        {
                            chain.AddAction(action, new UnitContext(sourceTile.Location), new UnitContext(sourceLocation));
                        }
                        chain.AddAction(this, new UnitContext(sourceLocation), new UnitContext(location));

                        dic[location] = chain;
                    }
                }
            }

            return dic;
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            var attacking_unit_accuracy = GetAccuracy(state, context, sourceTile, targetTile);
            var defending_unit_concealment = GetUnitConcealment(state, context, sourceTile, targetTile);
            var attacking_unit_attack = GetDamage(state, context, sourceTile, targetTile);
            var defending_unit_armor = GetUnitArmor(state, context, sourceTile, targetTile);
            var defending_terrain_defense = GetTerrainDefense(state, context, sourceTile, targetTile);
            var defending_terrain_concealment = GetTerrainConcealment(state, context, sourceTile, targetTile);
            var defending_height_modifier = GetUnitConcealmentHeightDifferenceModifier(state, context, sourceTile, targetTile);
            var attacking_unit_terrain_attack = GetTerrainDamage(state, context, sourceTile, targetTile);

            var subunits = GetSubunits(state, context, sourceTile, targetTile);
            var attacking_unit_actual_accuracy = GetActualAccuracy(state, context, sourceTile, targetTile, attacking_unit_accuracy, defending_unit_concealment);
            var damage_penetration = GetDamagePenetration(state, context, sourceTile, targetTile, defending_unit_armor);

            if(defending_unit_armor >= ArmorPenetration)
            {
                return new List<Modifier>()
                {
                    new Modifier("unit_damage", 0,
                        new ModifierList("attack_no_damage_from_armor", ArmorPenetration, defending_unit_armor)
                    )
                };
            }

            var accuracy_formual = new ModifierForumla("potential_damage", (attacking_unit_attack * (attacking_unit_actual_accuracy / 100.0)),
                        new Modifier("attacking_unit_base_damage", attacking_unit_attack),
                        ModifierForumla.OPERATOR_MULTIPLY,
                        new ModifierForumla("attacking_unit_actual_accuracy", attacking_unit_actual_accuracy / 100.0,
                            new Modifier("base_accuracy", BaseAccuracy / 100.0),
                            ( sourceTile.Unit.UnitType.EffectedByTerrainModifiers ?
                                new Modifier("terrain_accuracy_modifier", sourceTile.Terrain.TerrainType.AccuracyModifier / 100.0)
                                :
                                new Modifier("terrain_accuracy_modifier_not_effected_by_terrain_modifiers", null)
                            ),
                            ( sourceTile.Unit.UnitType.EffectedByTerrainModifiers || targetTile.Unit.UnitType.EffectedByTerrainModifiers ?
                                new Modifier("terrain_high_difference_modifier", -(defending_height_modifier / 100.0))
                                :
                                new Modifier("terrain_high_difference_modifier_not_effected_by_terrain_modifiers", null)
                            ),
                            new Modifier("base_unit_concealment", -(targetTile.Unit.UnitType.Concealment / 100.0)),
                            ( targetTile.Unit.UnitType.EffectedByTerrainModifiers ?
                                new Modifier("terrain_concealment_modifier", -(targetTile.Terrain.TerrainType.ConcealmentModifier / 100.0))
                                :
                                new Modifier("terrain_concealment_modifier_not_effected_by_terrain_modifiers", null)
                            ),
                            
                            ( targetTile.Unit.UnitType.EffectedByTerrainModifiers ?
                                new Modifier("dig_in_concealment_bonus", -(GetUnitConcealmentDigInBonus(state, context, sourceTile, targetTile) / 100.0))
                                :
                                new Modifier("dig_in_concealment_bonus_not_effected_by_dig_in_modifiers", null)
                            )
                        )
                    );

            if (attacking_unit_actual_accuracy <= 0.0)
            {
                return new List<Modifier>()
                {
                    new Modifier("unit_damage", 0,
                        accuracy_formual
                    )
                };
            }


            return new List<Modifier>()
            {
                new ModifierForumla(
                    "unit_damage",
                    CalculateDamage(state, context, sourceTile, targetTile, attacking_unit_accuracy, defending_unit_concealment, attacking_unit_attack, defending_unit_armor),

                    accuracy_formual,
                    ModifierForumla.OPERATOR_MINUS,
                    ( IsArmorAnImpedance(state, context, sourceTile, targetTile, defending_unit_armor) ?
                        new Modifier("damage_blocked_by_armor", (attacking_unit_attack * (attacking_unit_actual_accuracy / 100.0) * (1.0 - damage_penetration)),
                            new Modifier("attackers_pen", ArmorPenetration),
                            new Modifier("defenders_armor", defending_unit_armor),
                            new Modifier("damage_penetration", damage_penetration)
                        )
                        :
                        new ModifierList("damage_blocked_by_armor_overmatched", ArmorPenetration, defending_unit_armor)
                    )
                    //new Modifier("defending_unit_armor", -effective_armor)
                    //ModifierForumla.OPERATOR_MULTIPLY,
                    //new ModifierForumla("defending_unit_armor_effectiveness", effective_armor,
                    //    ModifierForumla.OPERATOR_LEFT_PARENTHESE,
                    //    new Modifier("attacking_unit_armor_penetration", ArmorPenetration),
                    //    ModifierForumla.OPERATOR_MINUS,
                    //    new Modifier("base_unit_armor", defending_unit_armor),
                    //    ModifierForumla.OPERATOR_RIGHT_PARENTHESE,
                    //    ModifierForumla.OPERATOR_DIVIDE,
                    //    new Modifier("attacking_unit_armor_penetration", ArmorPenetration)
                    //)
                )
                //),
                //new ModifierForumla(
                //    "terrain_damage",
                //    CalculateDamage(state, context, sourceTile, targetTile, attacking_unit_accuracy, defending_unit_concealment, attacking_unit_terrain_attack, defending_terrain_defense),
                //    new Modifier( "attacking_unit_damage_vs_terrain", ),
                //    { "defending_terrain_defense", GetTerrainDefense(state, context, sourceTile, targetTile) }
                //)
            };
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(TargetableMovementTypes != null);
            Contract.Invariant(SuppliesNeeded != null);
            Contract.Invariant(ActionsNeeded > -1);
            Contract.Invariant(MinimumRange > -1);
            Contract.Invariant(MaximumRange > -1);
        }
    }
}
