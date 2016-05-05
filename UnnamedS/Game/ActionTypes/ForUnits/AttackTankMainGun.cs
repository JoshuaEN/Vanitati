using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackTankMainGun : AttackBase
    {
        public override int BaseAccuracy { get; } = 70;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = TARGETABLE_LAND_VEHICLE_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 1;
        public override int MaximumRange { get; } = 1;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.LargeCaliberTankGuns;
        public override double DamagePerSubunit { get; } = 5;
        public override double TerrainDamagePerSubunit { get; } = 0;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 1 }
        };

        private AttackTankMainGun() : base("tank_main_gun") { }
        public static AttackTankMainGun Instance { get; } = new AttackTankMainGun();
    }
}
