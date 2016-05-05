using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public class AttackHeavyMachineGun : AttackBase
    {
        public override int BaseAccuracy { get; } = 90;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = TARGETABLE_LAND_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 1;
        public override int MaximumRange { get; } = 1;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.HeavyMachineGuns;
        public override double DamagePerSubunit { get; } = 2;
        public override double TerrainDamagePerSubunit { get; } = 0;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bullets.Instance, 1 }
        };

        private AttackHeavyMachineGun() : base("heavy_machine_gun") { }
        protected AttackHeavyMachineGun(string key) : base(key) { }
        public static AttackHeavyMachineGun Instance { get; } = new AttackHeavyMachineGun();
    }
}
