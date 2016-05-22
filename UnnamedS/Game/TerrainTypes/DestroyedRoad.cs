using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class DestroyedRoad : DestroyedLandStructure
    {
        private DestroyedRoad() : base("destroyed_road") { }
        public static DestroyedRoad Instance { get; } = new DestroyedRoad();

        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Natural;
    }
}
