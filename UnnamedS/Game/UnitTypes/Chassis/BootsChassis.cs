using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public abstract class BootsChassis : UnitType
    {
        public override MovementType MovementType { get; } = MovementTypes.Boots.Instance;
        public override int MaxMovement { get; } = 3;

        public override double MaxArmor { get; } = ArmorProtectionFrom.Nothing;
        public override int Concealment { get; } = 10;

        public override IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
        };
        public override IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Rations.Instance, 1 }
        };

        protected BootsChassis(string key) : base(key) { }
    }
}
