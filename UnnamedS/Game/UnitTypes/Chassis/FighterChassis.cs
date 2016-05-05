using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public abstract class FighterChassis : UnitType
    {
        public override MovementType MovementType { get; } = MovementTypes.Propeller.Instance;
        public override int MaxMovement { get; } = 20;

        public override double MaxArmor { get; } = ArmorProtectionFrom.SmallArms;
        public override int Concealment { get; } = 0;

        public override IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Kerosene.Instance, 1 }
        };
        public override IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Kerosene.Instance, 1 }
        };

        public override bool EffectedByTerrainModifiers { get; } = false;

        protected FighterChassis(string key) : base(key) { }
    }
}
