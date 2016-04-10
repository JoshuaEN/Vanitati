using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    public abstract class AttackBase : ActionType
    {
        public HashSet<MovementType> TargetableMovementTypes { get; }
        public int AttacksNeeded { get; } = -1;
        public int MinimumRange { get; } = -1;
        public int MaximumRange { get; } = -1;
        public Dictionary<SupplyType, int> SuppliesNeeded { get; }

        protected AttackBase(string key, int maximumRange, HashSet<MovementType> targetableMovementTypes, Dictionary<SupplyType, int> suppliesNeeded = null, int minimumRange = 1, int attacksNeeded = 1) : base("attack_" + key, ActionTarget.AnyOtherUnit, ActionTriggers.None, true)
        {
            Contract.Requires<ArgumentNullException>(null != targetableMovementTypes);

            TargetableMovementTypes = targetableMovementTypes;
            SuppliesNeeded = suppliesNeeded ?? new Dictionary<SupplyType, int>(0);
            AttacksNeeded = attacksNeeded;
            MaximumRange = maximumRange;
            MinimumRange = minimumRange;
        }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile, Tile targetTile)
        {
            if (base.CanPerformOn(state, context, sourceTile, targetTile) == false)
                return false;

            var unit = sourceTile.Unit;

            if (HasRequiredResources(state, sourceTile) == false)
                return false;

            if (CanTargetMovementType(targetTile.Unit.UnitType.MovementType) == false)
                return false;

            if (state.LocationsAroundPoint(sourceTile.Location, MinimumRange, MaximumRange).Contains(targetTile.Location) == false)
                return false;

            return true;
        }

        protected bool HasRequiredResources(IReadOnlyBattleGameState state, Tile sourceTile)
        {
            var unit = sourceTile.Unit;

            if (unit == null)
                return false;

            if (unit.Attacks < AttacksNeeded)
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

        protected bool CanTargetMovementType(MovementType type)
        {
            return TargetableMovementTypes.Contains(type);
        }

        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, ActionContext context, Tile sourceTile)
        {
            var dic = new Dictionary<Location, ActionChain>();

            if (sourceTile.Unit == null || HasRequiredResources(state, sourceTile) == false)
                return dic;

            var sourceLocations = sourceTile.Unit.GetAvailableMovement(state);
            sourceLocations.Add(sourceTile.Location, null);

            foreach (var kp in  sourceLocations)
            {
                var sourceLocation = kp.Key;
                var action = kp.Value;
                var currentTile = state.GetTile(sourceLocation);
                if (currentTile == null)
                    continue;

                foreach (var location in state.LocationsAroundPoint(sourceLocation, MinimumRange, MaximumRange))
                {
                    ActionChain chain;
                    if (dic.TryGetValue(location, out chain) && (chain.Length == 1 || action != null))
                        continue;

                    var targetTile = state.GetTile(location);
                    if (targetTile == null || targetTile.Unit == null || location == sourceTile.Location)
                        continue;

                    if (targetTile.Unit.CommanderID != context.CommanderID && CanTargetMovementType(targetTile.Unit.UnitType.MovementType))
                    {
                        chain = new ActionChain();

                        if (action != null)
                        {
                            chain.AddAction(action, sourceTile.Location, sourceLocation);
                        }
                        chain.AddAction(this, sourceLocation, location);

                        dic[location] = chain;
                    }
                }
            }

            return dic;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(TargetableMovementTypes != null);
            Contract.Invariant(SuppliesNeeded != null);
            Contract.Invariant(AttacksNeeded > -1);
            Contract.Invariant(MinimumRange > -1);
            Contract.Invariant(MaximumRange > -1);
        }
    }
}
