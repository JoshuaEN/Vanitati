using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackAntiAircraftCannonVsAir : AttackAntiAircraftCannonBase
    {
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.AIR_VEHICLE_MOVEMENT_TYPES;
        public override int MaximumRange { get; } = 2;
        public override int MinimumRange { get; } = 1;

        public override bool CanRetaliate { get; } = true;

        private AttackAntiAircraftCannonVsAir() : base("anti_aircraft_cannon_vs_air") { }
        public static AttackAntiAircraftCannonVsAir Instance { get; } = new AttackAntiAircraftCannonVsAir();
    }
}
