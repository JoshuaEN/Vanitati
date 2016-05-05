using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnnamedStrategyGame.Game.Action.TargetContextBase;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CommanderTargetOtherContext : TargetContextBase<CommanderContext, OtherContext, ActionTypes.CommanderAction.ActionTriggers>
    {
        public Commander SourceCommander { get; }

        public CommanderTargetOtherContext(IReadOnlyBattleGameState state, ActionContext context) : base(state, context, Load.Source)
        {
            SourceCommander = state.GetCommander(Source.CommanderID);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != SourceCommander || Loaded.HasFlag(Load.Source) == false);
        }
    }
}
