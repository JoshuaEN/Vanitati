using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;
using UnnamedStrategyGame.Game.Properties;

namespace UnnamedStrategyGame.Game
{
    public sealed class Unit : IPropertyContainer
    {
        public int UnitID { get; } = -1;
        public UnitType UnitType { get; set; }
        public Location Location { get; set; }
        public IDictionary<SupplyType, int> Supplies { get; set; }
        public int CommanderID { get; set; } = -1;
        public int Movement { get; set; } = -1;
        public int Attacks { get; set; } = -1;
        public int Health { get; set; } = -1;

        [Newtonsoft.Json.JsonConstructor]
        public Unit(int unitID, UnitType unitType, Location location, int commanderID)
        {
            Contract.Requires<ArgumentNullException>(unitType != null);
            Contract.Requires<ArgumentNullException>(location != null);
            Contract.Requires<ArgumentException>(unitID > -1);
            Contract.Requires<ArgumentException>(commanderID > -1);

            UnitID = unitID;
            UnitType = unitType;
            Location = location;
            Supplies = UnitType.SupplyLimits.ToDictionary(kp => kp.Key, kp => kp.Value);
            CommanderID = commanderID;
            Movement = UnitType.MaxMovement;
            Attacks = UnitType.MaxAttacks;
            Health = UnitType.MaxHealth;
        }

        public Unit(IDictionary<string, object> values)
        {
            SetProperties(values);
        }

        public IDictionary<Location, ActionType> GetAvailableMovement(IReadOnlyBattleGameState state)
        {
            Contract.Requires<ArgumentNullException>(null != state);

            var sourceTile = state.GetTile(Location);
            var dic = new Dictionary<Location, ActionType>();

            foreach(var action in UnitType.Actions.Where(a => a.CausesMovement && a.AvailableDuringTurn))
            {
                foreach(var location in (action as ActionTypes.ICausesMovement).GetRemainingMovement(state, new Game.Action.ActionContext(CommanderID, ActionType.ActionTriggers.None), sourceTile).Keys)
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
            Contract.Invariant(Attacks > -1);
            Contract.Invariant(Health > -1);
        }

    }
}
