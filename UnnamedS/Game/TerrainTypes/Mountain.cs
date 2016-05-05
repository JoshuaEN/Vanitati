using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Mountain : Land
    {
        public override TerrainHeight Height { get; } = TerrainHeight.Elevated;
        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Treacherous;
        public override bool CanBePillage { get; } = false;

        private Mountain() : base("mountain") { }
        public static Mountain Instance { get; } = new Mountain();
    }
}
