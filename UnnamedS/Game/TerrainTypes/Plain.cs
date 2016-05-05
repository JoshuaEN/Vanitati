using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Plain : Land
    {
        public override TerrainHeight Height { get { return TerrainHeight.Normal; } }
        public override TerrainDifficulty Difficultly { get { return TerrainDifficulty.Natural; } }

        private Plain() : base("plain") { }
        public static Plain Instance { get; } = new Plain();
    }
}
