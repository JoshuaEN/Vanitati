using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public class Tile
    {
        public Unit Unit { get; }
        public Terrain Terrain { get; }

        public Tile(Terrain terrain, Unit unit)
        {
            Terrain = terrain;
            Unit = unit;
        }

        public static Tile NullTile { get; } = new Tile(null, null);
    }
}
