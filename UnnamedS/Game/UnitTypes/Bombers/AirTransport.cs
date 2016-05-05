using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes.Bombers
{
    public sealed class AirTransport : BomberChassis
    {
        public override int BuildCost { get; } = 25000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Kerosene.Instance, 200 }
        };

        private AirTransport() : base("air_transport") { }
        public static AirTransport Instance { get; } = new AirTransport();
    }
}
