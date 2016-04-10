using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    public abstract class UnitType : BaseType
    {
        public MovementType MovementType { get; }
        public IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; }
        public IReadOnlyList<ActionType> Actions { get; }
        public int MaxMovement { get; } = -1;
        public int MaxAttacks { get; } = -1;
        public int MaxHealth { get; } = -1;

        public static IReadOnlyDictionary<string, UnitType> TYPES { get; }

        static UnitType()
        {
            TYPES = BuildTypeListing<UnitType>("UnnamedStrategyGame.Game.UnitTypes");
        }

        protected UnitType(
            string key,
            MovementType movementType, 
            List<ActionType> actions, 
            Dictionary<SupplyType, int> supplyLimits,
            int maxMovement,
            int maxAttacks = 1,
            int maxHealth = 10) : base("unit_" + key)
        {
            Contract.Requires<ArgumentNullException>(key != null);
            Contract.Requires<ArgumentNullException>(movementType != null);
            Contract.Requires<ArgumentNullException>(actions != null);
            Contract.Requires<ArgumentNullException>(supplyLimits != null);
            Contract.Requires<ArgumentException>(maxMovement > -1);
            Contract.Requires<ArgumentException>(maxAttacks > -1);
            Contract.Requires<ArgumentException>(maxHealth > -1);

            actions.Insert(0, ActionTypes.Move.Instance);
            actions.Add(ActionTypes.ReplenishUnitTurnResources.Instance);

            MovementType = movementType;
            Actions = actions;
            SupplyLimits = supplyLimits;
            MaxMovement = maxMovement;
            MaxAttacks = maxAttacks;
            MaxHealth = maxHealth;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(MovementType != null);
            Contract.Invariant(SupplyLimits != null);
            Contract.Invariant(Actions != null);
            Contract.Invariant(MaxMovement > -1);
            Contract.Invariant(MaxHealth > -1);
        }

    }
}
