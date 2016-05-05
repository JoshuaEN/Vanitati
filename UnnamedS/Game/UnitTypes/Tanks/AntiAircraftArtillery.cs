using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class AntiAircraftArtillery : TankChassis
    {

        public override int BuildCost { get; } = 15000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackAntiAircraftCannonVsGround.Instance,
            ActionTypes.ForUnits.AttackAntiAircraftCannonVsAir.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 10 },
            { SupplyTypes.Diesel.Instance, 60 },
            { SupplyTypes.Rations.Instance, 40 }
        };

        private AntiAircraftArtillery() : base("anti_aircraft_artillery") { }

        public static AntiAircraftArtillery Instance { get; } = new AntiAircraftArtillery();

    }
}
