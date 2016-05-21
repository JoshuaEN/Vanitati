using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class VictoryPoint : Land
    {
        private VictoryPoint() : base("victory_point") { }
        public static VictoryPoint Instance { get; } = new VictoryPoint();

        public override bool CanCapture { get; } = true;
        public override bool IsVictoryPoint { get; } = true;

        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Tamed;
        public override TerrainHeight Height { get; } = TerrainHeight.Normal;
    }
}
