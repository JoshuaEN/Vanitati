using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class Infantry : BootsChassis
    {
        public override int BuildCost { get; } = 1000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackRifle.Instance,
            ActionTypes.ForUnits.Capture.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            {SupplyTypes.Rations.Instance, 24 },
            {SupplyTypes.Bullets.Instance, 10 }
        };

        private Infantry() : base( "infantry") { }
        public static Infantry Instance { get; } = new Infantry();
    }
}
