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
    public class TileTargetOtherContext<TSource, TTrigger> : TargetContextZeroArgs<TSource, TTrigger> where TSource:TileContext
    {
        public Tile SourceTile { get; }

        protected TileTargetOtherContext(IReadOnlyBattleGameState state, ActionContext context) : base(state, context, Load.Source)
        {
            SourceTile = state.GetTile(Source.Location);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != SourceTile || Loaded.HasFlag(Load.Source) == false);
        }
    }
}
