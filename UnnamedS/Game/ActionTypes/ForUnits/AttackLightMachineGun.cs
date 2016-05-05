using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackLightMachineGun : AttackBase
    {
        public override int BaseAccuracy { get; } = 70;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = TARGETABLE_LAND_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 1;
        public override int MaximumRange { get; } = 1;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.LightMachineGuns;
        public override double DamagePerSubunit { get; } = 2.8;
        public override double TerrainDamagePerSubunit { get; } = 0;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bullets.Instance, 1 }
        };

        private AttackLightMachineGun() : base("light_machine_gun") { }
        public static AttackLightMachineGun Instance { get; } = new AttackLightMachineGun();
    }
}
