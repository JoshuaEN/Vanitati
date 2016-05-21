using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class Road : Land
    {
        private Road() : base("road") { }
        public static Road Instance { get; } = new Road();
        public override bool CanBePillage { get; } = true;
        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Paved;

        public override TerrainHeight Height { get; } = TerrainHeight.Normal;
    }
}
