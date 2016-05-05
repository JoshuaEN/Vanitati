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
    public abstract class TileTargetCommanderContext<TSource, TTrigger> : TargetContextBase<TSource, CommanderContext, TTrigger> where TSource:TileContext
    {
        public Tile SourceTile { get; } 
        public Commander TargetCommander { get; }

        protected TileTargetCommanderContext(IReadOnlyBattleGameState state, ActionContext context, Load load) : base(state, context, load)
        {
            if (load.HasFlag(Load.Source))
                SourceTile = state.GetTile(Source.Location);
            if (load.HasFlag(Load.Target))
                TargetCommander = state.GetCommander(Target.CommanderID);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != SourceTile || Loaded.HasFlag(Load.Source) == false);
            Contract.Invariant(null != TargetCommander || Loaded.HasFlag(Load.Target) == false);
        }
    }
}
