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
    public class CommanderTargetCommanderContext : TargetContextOneArg<CommanderContext, int, ActionTypes.CommanderAction.ActionTriggers>
    {
        public Commander SourceCommander { get; }
        public Commander TargetCommander { get; }

        public CommanderTargetCommanderContext(IReadOnlyBattleGameState state, ActionContext context, Load load = Load.Source | Load.Target) : base(state, context, load)
        {
            if (load.HasFlag(Load.Source))
                SourceCommander = state.GetCommander(Source.CommanderID);
            if (load.HasFlag(Load.Target))
                TargetCommander = state.GetCommander(TargetValue);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != SourceCommander || Loaded.HasFlag(Load.Source) == false);
            Contract.Invariant(null != TargetCommander || Loaded.HasFlag(Load.Target) == false);
        }
    }
}
