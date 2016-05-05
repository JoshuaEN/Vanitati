using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.MovementTypes
{
    public sealed class Wheels : MovementType
    {
        public override IReadOnlyList<TerrainType.TerrainClassifications> TraversableClassifications { get; } = new List<TerrainType.TerrainClassifications>()
        {
            TerrainType.TerrainClassifications.Land
        };

        public override IReadOnlyList<TerrainType.TerrainDifficulty> TraversableDifficulties { get; } = new List<TerrainType.TerrainDifficulty>()
        {
            TerrainType.TerrainDifficulty.Paved,
            TerrainType.TerrainDifficulty.Tamed,
            TerrainType.TerrainDifficulty.Natural,
            TerrainType.TerrainDifficulty.Rough
        };

        public override int GetMovementCost(TerrainType terrainType)
        {
            switch (terrainType.Difficultly)
            {
                case TerrainType.TerrainDifficulty.Treacherous:
                    return Int16.MaxValue;
                case TerrainType.TerrainDifficulty.Rough:
                    return 4;
                case TerrainType.TerrainDifficulty.Natural:
                case TerrainType.TerrainDifficulty.Tamed:
                    return 2;
                default:
                    return 1;
            }
        }

        private Wheels() : base("wheels") { }
        public static Wheels Instance { get; } = new Wheels();
    }
}
