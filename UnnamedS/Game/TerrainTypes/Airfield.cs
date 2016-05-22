using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Airfield : LandStructure
    {
        public override IReadOnlyList<TerrainAction> Actions { get; } = new List<TerrainAction>()
            {
                ActionTypes.ForTerrain.BuildAirUnit.Instance,
                ActionTypes.ForTerrain.RepairAndResupply.Instance,
                ActionTypes.ForTerrain.OnTerrainHealthChanged.Instance
            };

        public override int MaxHealth { get; } = 25;

        public override IReadOnlyDictionary<SupplyType, int> ResuppliesPerTurn { get; } = SupplyType.TYPES.ToDictionary(kp => kp.Value, kp => 999);
        public override IReadOnlyDictionary<MovementType, int> RepairsPerTurn { get; } = MovementType.AIR_VEHICLE_MOVEMENT_TYPES.ToDictionary(mv => mv, mv => 2);

        public override TerrainType BecomesWhenDestroyed { get; } = DestroyedAirfield.Instance;

        private Airfield() : base("airfield") { }
        public static Airfield Instance { get; } = new Airfield();
    }
}
