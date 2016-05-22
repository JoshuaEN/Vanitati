using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class Runway : LandStructure
    {
        public override TerrainDifficulty Difficultly { get; } = TerrainDifficulty.Paved;

        public override TerrainHeight Height { get; } = TerrainHeight.Normal;

        public override int ConcealmentModifier { get; } = -20;

        public override IReadOnlyList<TerrainAction> Actions { get; } = new List<TerrainAction>()
            {
                ActionTypes.ForTerrain.RepairAndResupply.Instance,
                ActionTypes.ForTerrain.OnTerrainHealthChanged.Instance
            };

        public override int MaxHealth { get; } = 20;

        public override IReadOnlyDictionary<SupplyType, int> ResuppliesPerTurn { get; } = SupplyType.TYPES.ToDictionary(kp => kp.Value, kp => 999);
        public override bool CanSupply { get; } = true;

        public override TerrainType BecomesWhenDestroyed { get; } = DestroyedRunway.Instance;

        private Runway() : base("runway") { }
        public static Runway Instance { get; } = new Runway();
    }
}
