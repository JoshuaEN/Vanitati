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
    public class TileTargetTileContext<TSource, TTarget> : TargetContextBase<TSource, TileContext, TTarget> where TSource:TileContext
    {
        public Tile SourceTile { get; }
        public Tile TargetTile { get; }

        protected TileTargetTileContext(IReadOnlyBattleGameState state, ActionContext context, Load load) : base(state, context, load)
        {
            if (load.HasFlag(Load.Source))
                SourceTile = state.GetTile(Source.Location);
            if (load.HasFlag(Load.Target))
                TargetTile = state.GetTile(Target.Location);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != SourceTile || Loaded.HasFlag(Load.Source) == false);
            Contract.Invariant(null != TargetTile || Loaded.HasFlag(Load.Target) == false);
        }
    }
}
