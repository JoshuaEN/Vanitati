﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class AttackSaturationBombing : AttackBase
    {
        public override int BaseAccuracy { get; } = 30;
        public override IReadOnlyList<MovementType> TargetableMovementTypes { get; } = MovementType.LAND_MOVEMENT_TYPES;
        public override int MinimumRange { get; } = 1;
        public override int MaximumRange { get; } = 1;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.Bombs;
        public override double DamagePerSubunit { get; } = 25;
        public override double TerrainDamagePerSubunit { get; } = 40;
        public override int MaxSubunits { get; } = 2;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bombs.Instance, 1 }
        };

        private AttackSaturationBombing() : base("saturation_bombing") { }
        public static AttackSaturationBombing Instance { get; } = new AttackSaturationBombing();
    }
}
