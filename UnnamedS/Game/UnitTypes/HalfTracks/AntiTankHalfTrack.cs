using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public class AntiTankHalfTrack : HalfTrackChassis
    {
        public override int BuildCost { get; } = 9000;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackAntiTankCannon.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Shells.Instance, 8 },
            { SupplyTypes.Diesel.Instance, 80 },
            { SupplyTypes.Rations.Instance, 50 }
        };

        private AntiTankHalfTrack() : base("anti_tank_half_track") { }
        public static AntiTankHalfTrack Instance { get; } = new AntiTankHalfTrack();
    }
}
