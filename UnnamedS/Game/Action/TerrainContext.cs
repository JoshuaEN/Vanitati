using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TerrainContext : TileContext
    {
        public override ActionType.Category ActionCategory
        {
            get
            {
                return ActionType.Category.Terrain;
            }
        }

        public TerrainContext(Location location) : base(location) { }
    }
}
