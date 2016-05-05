using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public abstract class CarChassis : UnitType
    {
        public override MovementType MovementType { get; } = MovementTypes.Wheels.Instance;
        public override int MaxMovement { get; } = 14;

        public override double MaxArmor { get; } = 0.5;
        public override int Concealment { get; } = 3;

        public override IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Diesel.Instance, 1 }
        };
        public override IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Rations.Instance, 1 }
        };

        protected CarChassis(string key) : base(key) { }
    }
}
