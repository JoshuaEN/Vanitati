using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.MovementTypes
{
    public sealed class Propeller : MovementType
    {
        public override IReadOnlyList<TerrainType.TerrainClassifications> TraversableClassifications { get; } = new List<TerrainType.TerrainClassifications>()
        {
            TerrainType.TerrainClassifications.Land,
            TerrainType.TerrainClassifications.River,
            TerrainType.TerrainClassifications.Sea
        };

        public override IReadOnlyList<TerrainType.TerrainDifficulty> TraversableDifficulties { get; } = new List<TerrainType.TerrainDifficulty>(0) { };

        public override int GetMovementCost(TerrainType terrainType)
        {
            return 1;
        }

        private Propeller() : base("propeller") { }
        public static Propeller Instance { get; } = new Propeller();
    }
}
