using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TileContext : SourceContext
    {
        public Location Location { get; }

        public TileContext(Location location)
        {
            Contract.Requires<ArgumentNullException>(null != location);

            Location = location;
        }

        public Tile GetTile(IReadOnlyBattleGameState state)
        {
            Contract.Requires<ArgumentNullException>(null != state);

            return state.GetTile(Location);
        }
    }
}
