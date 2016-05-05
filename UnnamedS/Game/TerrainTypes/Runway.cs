using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Runway : Land
    {
        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Paved;

        public override TerrainHeight Height { get; } = TerrainHeight.Normal;

        public override int ConcealmentModifier { get; } = -20;

        private Runway() : base("runway") { }
        public static Runway Instance { get; } = new Runway();
    }
}
