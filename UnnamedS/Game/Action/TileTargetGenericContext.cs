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
    public abstract class TileTargetGenericContext<TSource, T, TTrigger> : TargetContextOneArg<TSource, T, TTrigger> where TSource:TileContext
    {
        public Tile SourceTile { get; }

        protected TileTargetGenericContext(IReadOnlyBattleGameState state, ActionContext context, Load load) : base(state, context, load)
        {
            if(load.HasFlag(Load.Source))
                SourceTile = state.GetTile(Source.Location);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != SourceTile || Loaded.HasFlag(Load.Source) == false);
            Contract.Invariant(null != TargetValue || Loaded.HasFlag(Load.Target) == false);
        }
    }
}
