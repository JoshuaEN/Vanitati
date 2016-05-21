using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackArtilleryBarrage : AttackBase
    {
        public override int BaseAccuracy { get; } = 20;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.LAND_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 2;
        public override int MaximumRange { get; } = 3;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.LandArtillery;
        public override double DamagePerSubunit { get; } = 3;
        public override double TerrainDamagePerSubunit { get; } = 6;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 1 }
        };

        private AttackArtilleryBarrage() : base("artillery_barrage") { }
        public static AttackArtilleryBarrage Instance { get; } = new AttackArtilleryBarrage();
    }
}
