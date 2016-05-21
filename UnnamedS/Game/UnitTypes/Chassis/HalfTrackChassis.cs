using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public abstract class HalfTrackChassis : UnitType
    {
        public override MovementType MovementType { get; } = MovementTypes.HalfTrack.Instance;
        public override int MaxMovement { get; } = 6;

        public override double MaxArmor { get; } = ArmorProtectionFrom.LightMachineGuns;
        public override int Concealment { get; } = 2;

        public override IReadOnlyDictionary<SupplyType, int> MovementSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Diesel.Instance, 1 }
        };
        public override IReadOnlyDictionary<SupplyType, int> TurnSupplyUsage { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Rations.Instance, 1 }
        };

        protected HalfTrackChassis(string key) : base(key) { }
    }
}
