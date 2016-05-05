using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.MovementTypes
{
    public sealed class Boots : MovementType
    {
        public override IReadOnlyList<TerrainType.TerrainClassifications> TraversableClassifications { get; } = new List<TerrainType.TerrainClassifications>()
        {
            TerrainType.TerrainClassifications.Land,
            TerrainType.TerrainClassifications.River
        };

        public override IReadOnlyList<TerrainType.TerrainDifficulty> TraversableDifficulties { get; } = new List<TerrainType.TerrainDifficulty>() { };

        public override int GetMovementCost(TerrainType terrainType)
        {
            switch(terrainType.Difficultly)
            {
                case TerrainType.TerrainDifficulty.Treacherous:
                    return 3;
                case TerrainType.TerrainDifficulty.Rough:
                    return 2;
                default:
                    return 1;
            }
        }

        private Boots() : base("boots") { }
        public static Boots Instance { get; } = new Boots();
    }
}
