using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class Forest : Land
    {
        private Forest() : base("forest") { }
        public static Forest Instance { get; } = new Forest();

        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Rough;

        public override TerrainHeight Height { get; } = TerrainHeight.Normal;
    }
}
