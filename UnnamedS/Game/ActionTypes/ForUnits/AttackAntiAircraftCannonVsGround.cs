using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackAntiAircraftCannonVsGround : AttackAntiAircraftCannonBase
    {
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.LAND_MOVEMENT_TYPES;
        public override int MaximumRange { get; } = 1;
        public override int MinimumRange { get; } = 1;

        private AttackAntiAircraftCannonVsGround() : base("anti_aircraft_cannon_vs_ground") { }
        public static AttackAntiAircraftCannonVsGround Instance { get; } = new AttackAntiAircraftCannonVsGround();
    }
}
