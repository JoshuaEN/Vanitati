using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class RocketArtillery : TruckChassis
    {
        public override int BuildCost { get; } = 20000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackRocketArtilleryBarrage.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Rockets.Instance, 8 },
            { SupplyTypes.Diesel.Instance, 65 },
            { SupplyTypes.Rations.Instance, 50 }
        };

        private RocketArtillery() : base("rocket_artillery") { }
        public static RocketArtillery Instance { get; } = new RocketArtillery();
    }
}
