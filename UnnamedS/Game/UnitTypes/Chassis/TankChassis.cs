using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public abstract class TankChassis : UnitType
    {
        public override MovementType MovementType { get; } = MovementTypes.Treads.Instance;
        public override int MaxMovement { get; } = 6;

        public override double MaxArmor { get; } = ArmorProtectionFrom.SmallCaliberTankGuns - 1;
        public override int Concealment { get; } = 0;

        public override IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Diesel.Instance, 1 }
        };
        public override IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Rations.Instance, 1 }
        };

        protected TankChassis(string key) : base(key) { }
    }
}
