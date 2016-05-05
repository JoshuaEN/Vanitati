using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class Artillery : TankChassis
    {
        public override double MaxArmor { get; } = ArmorProtectionFrom.SmallArms;

        public override int BuildCost { get; } = 16000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackArtilleryBarrage.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 6 },
            { SupplyTypes.Diesel.Instance, 55 },
            { SupplyTypes.Rations.Instance, 60 }
        };

        private Artillery() : base("artillery") { }

        public static Artillery Instance { get; } = new Artillery();
    }
}
