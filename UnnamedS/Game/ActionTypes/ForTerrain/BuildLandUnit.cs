using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    public sealed class BuildLandUnit : BuildUnit
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        private BuildLandUnit() : base(
            "build_land_unit",
            buildableTypes: UnitType.TYPES.Values.Where(t => t.MovementType.TraversableClassifications.Contains(TerrainType.TerrainClassifications.Land)).ToList()
        ) { }
        public static BuildLandUnit Instance { get; } = new BuildLandUnit();
    }
}
