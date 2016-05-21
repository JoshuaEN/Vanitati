using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class AntiAirHalfTrack : HalfTrackChassis
    {
        public override int BuildCost { get; } = 10000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackQuadHMGVsGround.Instance,
            ActionTypes.ForUnits.AttackQuadHMGVsAir.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bullets.Instance, 8 },
            { SupplyTypes.Diesel.Instance, 80 },
            { SupplyTypes.Rations.Instance, 50 }
        };

        private AntiAirHalfTrack() : base("anti_air_half_track") { }
        public static AntiAirHalfTrack Instance { get; } = new AntiAirHalfTrack();
    }
}
