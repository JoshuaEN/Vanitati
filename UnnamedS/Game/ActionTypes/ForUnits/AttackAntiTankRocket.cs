using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackAntiTankRocket : AttackBase
    {
        public override int BaseAccuracy { get; } = 40;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = TARGETABLE_LAND_VEHICLE_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 1;
        public override int MaximumRange { get; } = 1;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.AntiTankRockets;
        public override double DamagePerSubunit { get; } = 7;
        public override double TerrainDamagePerSubunit { get; } = 0;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Rockets.Instance, 1 }
        };

        private AttackAntiTankRocket() : base("anti_tank_rocket") { }
        public static AttackAntiTankRocket Instance { get; } = new AttackAntiTankRocket();
    }
}
