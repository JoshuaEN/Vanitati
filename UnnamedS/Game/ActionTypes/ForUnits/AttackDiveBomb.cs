using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackDiveBomb : AttackBase
    {
        public override int BaseAccuracy { get; } = 25;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = TARGETABLE_LAND_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 1;
        public override int MaximumRange { get; } = 1;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.Bombs;
        public override double DamagePerSubunit { get; } = 9;
        public override double TerrainDamagePerSubunit { get; } = 9;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bombs.Instance, 1 }
        };

        private AttackDiveBomb() : base("dive_bomb") { }
        public static AttackDiveBomb Instance { get; } = new AttackDiveBomb();
    }
}
