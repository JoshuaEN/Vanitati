using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackFighterHeavyMachineGun : AttackHeavyMachineGun
    {
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.LAND_MOVEMENT_TYPES.Concat(MovementType.AIR_VEHICLE_MOVEMENT_TYPES).ToList();

        public override int BaseAccuracy { get; }
        public override double DamagePerSubunit { get; }

        private AttackFighterHeavyMachineGun() : base("fighter_heavy_machine_gun")
        {
            BaseAccuracy = 20;
            DamagePerSubunit = base.DamagePerSubunit * 6;
        }

        new public static AttackFighterHeavyMachineGun Instance { get; } = new AttackFighterHeavyMachineGun();
    }
}
