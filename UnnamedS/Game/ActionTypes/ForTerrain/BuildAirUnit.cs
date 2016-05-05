using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    public sealed class BuildAirUnit : BuildUnit
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        private BuildAirUnit() : base(
            "build_air_unit",
            buildableTypes: UnitType.TYPES.Values.Where(t => t.MovementType == MovementTypes.Propeller.Instance).ToList()
        ) { }
        public static BuildAirUnit Instance { get; } = new BuildAirUnit();
    }
}
