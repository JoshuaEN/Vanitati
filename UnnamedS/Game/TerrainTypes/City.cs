using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public class City : LandStructure
    {
        private City() : base("city") { }
        public static City Instance { get; } = new City();

        public override IReadOnlyList<TerrainAction> Actions { get; } = new List<TerrainAction>()
            {
                ActionTypes.ForTerrain.RepairAndResupply.Instance
            };

        public override IReadOnlyDictionary<SupplyType, int> ResuppliesPerTurn { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Rations.Instance, 999 },
            { SupplyTypes.Diesel.Instance, 999 },
            { SupplyTypes.Bullets.Instance, 999 }
        };

        public override bool CanRepair { get; } = false;
        //public override IReadOnlyDictionary<MovementType, int> RepairsPerTurn { get; } = MovementType.LAND_MOVEMENT_TYPES.ToDictionary(mv => mv, mv => 2);
    }
}
