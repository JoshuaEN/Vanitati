using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackQuadHMGVsAir : AttackQuadHMGBase
    {
        public override int MaximumRange { get; } = 2;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = TARGETABLE_AIR_VEHICLE_MOVEMENT_TYPES;

        public override bool CanRetaliate { get; } = true;

        private AttackQuadHMGVsAir() : base("quad_hmg_vs_air") { }
        new public static AttackQuadHMGVsAir Instance { get; } = new AttackQuadHMGVsAir();
    }
}
