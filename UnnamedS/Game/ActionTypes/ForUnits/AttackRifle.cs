using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackRifle : AttackBase
    {
        public override int BaseAccuracy { get; } = 75;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.LAND_MOVEMENT_TYPES;
        public override int MaximumRange { get; } = 1;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.SmallArms;
        public override double DamagePerSubunit { get; } = 2;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bullets.Instance, 1 }
        };

        private AttackRifle() : base("rifle") { }
        public static AttackRifle Instance { get; } = new AttackRifle();
    }
}
