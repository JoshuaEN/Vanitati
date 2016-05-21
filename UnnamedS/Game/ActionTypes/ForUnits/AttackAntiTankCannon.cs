using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public class AttackAntiTankCannon : AttackBase
    {
        public override int BaseAccuracy { get; } = 65;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.LAND_VEHICLE_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 1;
        public override int MaximumRange { get; } = 2;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.LargeCaliberTankGuns;
        public override double DamagePerSubunit { get; } = 2;
        public override double TerrainDamagePerSubunit { get; } = 0;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 1 }
        };

        private AttackAntiTankCannon() : base("anti_tank_cannon") { }
        public static AttackAntiTankCannon Instance { get; } = new AttackAntiTankCannon();
    }
}
