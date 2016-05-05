using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public abstract class BomberChassis : UnitType
    {
        public override MovementType MovementType { get; } = MovementTypes.Propeller.Instance;
        public override int MaxMovement { get; } = 14;

        public override double MaxArmor { get; } = ArmorProtectionFrom.SmallArms;
        public override int Concealment { get; } = -5;

        public override IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Kerosene.Instance, 1 }
        };
        public override IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Kerosene.Instance, 1 }
        };

        public override bool EffectedByTerrainModifiers { get; } = false;

        protected BomberChassis(string key) : base(key) { }
    }
}
