using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TileContext : Context
    {
        public Location Location { get; }

        public override ActionType.Category ActionCategory
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override ActionType.TargetCategory ActionTargetCategory
        {
            get
            {
                return ActionType.TargetCategory.Tile;
            }
        }

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
