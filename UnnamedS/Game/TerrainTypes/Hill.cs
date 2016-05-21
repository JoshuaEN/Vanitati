using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Hill : Land
    {
        public override TerrainHeight Height { get; } = TerrainHeight.Elevated;
        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Natural;

        private Hill() : base("hill") { }
        public static Hill Instance { get; } = new Hill();
    }
}
