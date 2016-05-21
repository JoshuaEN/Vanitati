using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.Properties;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class Unit : IPropertyContainer
    {
        [Newtonsoft.Json.JsonIgnore]
        private int _unitID = -1;
        public int UnitID
        {
            get { return _unitID; }
            set
            {
                Contract.Requires<ArgumentException>(value >= 0);
                if(_unitID != -1)
                    throw new NotSupportedException("Unit ID cannot be changed after being set");

                _unitID = value;
            }
        }
        public UnitType UnitType { get; private set; }
        public Location Location { get; private set; }
        public IDictionary<SupplyType, int> Supplies { get; private set; }
        public List<Unit> EmbarkedUnits { get; private set; } = new List<Unit>(0); 
        public int CommanderID { get; private set; } = -1;
        public int Movement { get; private set; } = -1;
        public int Actions { get; private set; } = -1;
        public int Health { get; private set; } = -1;
        public double Armor { get; private set; } = -1;
        public ActionInfo RepeatedAction { get; private set; }
        public bool Embarked { get; private set; }

        [Newtonsoft.Json.JsonConstructor]
        public Unit(int unitID, UnitType unitType, Location location, int commanderID, IDictionary<SupplyType, int> supplies = null, int? health = null, double? armor = null, ActionInfo repeatedAction = null, int movement = 0, int actions = 0, bool embarked = false, List<Unit> embarkedUnits = null)
        {
            Contract.Requires<ArgumentNullException>(unitType != null);
            Contract.Requires<ArgumentNullException>(location != null);
            Contract.Requires<ArgumentException>(unitID > -1);
            Contract.Requires<ArgumentException>(commanderID > -1);
            Contract.Requires<ArgumentException>(movement > -1);
            Contract.Requires<ArgumentException>(actions > -1);

            UnitID = unitID;
            UnitType = unitType;
            Location = location;
            Supplies = supplies ?? UnitType.SupplyLimits.ToDictionary(kp => kp.Key, kp => kp.Value);
            CommanderID = commanderID;
            Health = health ?? UnitType.MaxHealth;
            Armor = armor ?? UnitType.MaxArmor;
            RepeatedAction = repeatedAction ?? ActionTypes.ForUnits.NullUnitAction.ActionInfoInstance;
            Embarked = embarked;
            EmbarkedUnits = embarkedUnits ?? new List<Unit>(0);

            // Per a turn attributes
            Movement = movement;
            Actions = actions;
        }

        public Unit(IDictionary<string, object> values)
        {
            SetProperties(values);
        }

        public int GetEffectiveConcealment(IReadOnlyBattleGameState state, Terrain currentTerrain)
        {
            return GetConcealmentBase(state, currentTerrain) + GetConcealmentTerrainModifier(state, currentTerrain) + GetConcealmentDigInBonus(state, currentTerrain);
        }

        public int GetConcealmentDigInBonus(IReadOnlyBattleGameState state, Terrain currentTerrain)
        {
            if (UnitType.EffectedByTerrainModifiers)
                return ((int)Math.Round(GetConcealmentBase(state, currentTerrain) * (currentTerrain.DigIn * 2.0 / 5.0)));
            else
                return 0;
        }

        public int GetConcealmentBase(IReadOnlyBattleGameState state, Terrain currentTerrain)
        {
            return UnitType.Concealment;
        }

        public int GetConcealmentTerrainModifier(IReadOnlyBattleGameState state, Terrain currentTerrain)
        {
            if (UnitType.EffectedByTerrainModifiers)
                return currentTerrain.TerrainType.ConcealmentModifier;
            else
                return 0;
        }

        public IDictionary<Location, ActionType> GetAvailableMovement(IReadOnlyBattleGameState state)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            var sourceTile = state.GetTile(Location);

            return GetAvailableMovement(state, new UnitTargetTileContext(state, new ActionContext(state.CurrentCommander.CommanderID, ActionTypes.UnitAction.ActionTriggers.ManuallyByUser, new UnitContext(sourceTile.Location), new GenericContext(sourceTile.Location))), sourceTile);
        }

        public IDictionary<Location, ActionType> GetAvailableMovement(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        { 
            
            var dic = new Dictionary<Location, ActionType>();

            foreach(var action in UnitType.Actions.Where(a => a.CausesMovement && a.CanUserTrigger))
            {
                foreach(var location in (action as ActionTypes.ForUnits.ICausesMovement).GetRemainingMovement(state, context, sourceTile).Keys)
                {
                    if (dic.ContainsKey(location))
                        continue;

                    dic.Add(location, action);
                }
            }

            return dic;
        }

        public IDictionary<string, object> GetProperties()
        {
            return PropertyContainer.UNIT.GetProperties(this);
        }

        public IDictionary<string, object> GetWriteableProperties()
        {
            return PropertyContainer.UNIT.GetWriteableProperties(this);
        }

        public void SetProperties(IDictionary<string, object> values)
        {
            PropertyContainer.UNIT.SetProperties(this, values);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(UnitID > -1);
            Contract.Invariant(UnitType != null);
            Contract.Invariant(Location != null);
            Contract.Invariant(Supplies != null);
            Contract.Invariant(CommanderID > -1);
            Contract.Invariant(Movement > -1);
            Contract.Invariant(Actions > -1);
            Contract.Invariant(Health > -1);
            Contract.Invariant(Armor >= 0);
            Contract.Invariant(RepeatedAction != null);
        }

    }
}
