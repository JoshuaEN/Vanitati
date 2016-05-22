using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackRocketArtilleryBarrage : AttackBase
    {
        public override int BaseAccuracy { get; } = 15;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.LAND_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 3;
        public override int MaximumRange { get; } = 6;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.LandRocketArtillery;
        public override double DamagePerSubunit { get; } = 8;
        public override double TerrainDamagePerSubunit { get; } = 16;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Rockets.Instance, 1 }
        };

        private AttackRocketArtilleryBarrage() : base("rocket_artillery_barrage") { }
        public static AttackRocketArtilleryBarrage Instance { get; } = new AttackRocketArtilleryBarrage();
    }
}
