using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.TerrainTypes
{
    public sealed class Factory : LandStructure
    {
        public override IReadOnlyList<TerrainAction> Actions { get; } = new List<TerrainAction>()
            {
                ActionTypes.ForTerrain.BuildLandUnit.Instance,
                ActionTypes.ForTerrain.RepairAndResupply.Instance
            };

        public override int MaxHealth { get; } = 70;

        public override IReadOnlyDictionary<SupplyType, int> ResuppliesPerTurn { get; } = SupplyType.TYPES.ToDictionary(kp => kp.Value, kp => 999);
        public override IReadOnlyDictionary<MovementType, int> RepairsPerTurn { get; } = MovementType.LAND_MOVEMENT_TYPES.ToDictionary(mv => mv, mv => 2);

        private Factory() : base("factory") { }
        public static Factory Instance { get; } = new Factory();
    }
}
