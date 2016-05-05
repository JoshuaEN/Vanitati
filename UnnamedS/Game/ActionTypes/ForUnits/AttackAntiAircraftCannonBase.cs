using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public abstract class AttackAntiAircraftCannonBase : AttackBase
    {
        public override int BaseAccuracy { get; } = 52;
        public override double ArmorPenetration { get; } = UnitType.ArmorProtectionFrom.AutoCannon;
        public override double DamagePerSubunit { get; } = 4;
        public override double TerrainDamagePerSubunit { get; } = 0;
        public override Dictionary<SupplyType, int> SuppliesNeeded { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 1 }
        };

        protected AttackAntiAircraftCannonBase(string key) : base(key) { }
    }
}
