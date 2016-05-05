using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Game
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CommanderType : BaseType
    {
        public IReadOnlyList<CommanderAction> Actions { get; }

        protected CommanderType(
            string key,
            List<CommanderAction> actions = null,
            bool includeDefaultActions = true) : base("commander_" + key)
        {
            actions = actions ?? new List<CommanderAction>(0);

            if (includeDefaultActions)
            {
                actions.Add(ActionTypes.ForCommanders.ProvideCommanderEarnings.Instance);
                actions.Add(ActionTypes.ForCommanders.EndTurn.Instance);
            }

            Actions = actions;
        }

        public static IReadOnlyDictionary<string, CommanderType> TYPES { get; }
        static CommanderType()
        {
            TYPES = BuildTypeListing<CommanderType>("UnnamedStrategyGame.Game.CommanderTypes");
        }

        public override string ToString()
        {
            return Key;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Actions);
        }
    }
}
