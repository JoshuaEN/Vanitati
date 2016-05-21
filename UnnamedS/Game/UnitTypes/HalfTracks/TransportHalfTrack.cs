using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Game.UnitTypes
{
    public sealed class TransportHalfTrack : HalfTrackChassis
    {
        public override int BuildCost { get; } = 8000;

        public override IReadOnlyList<MovementType> EmbarkableMovementTypes { get; } = MovementType.LAND_INFANTRY_TYPES;
        public override int MaxEmbarkedUnits { get; } = 1;

        public override IReadOnlyList<UnitAction> Actions { get; } = new List<UnitAction>()
        {
            ActionTypes.ForUnits.Move.Instance,
            ActionTypes.ForUnits.AttackHeavyMachineGun.Instance,
            ActionTypes.ForUnits.UnloadEmbarkedUnit.Instance,
            ActionTypes.ForUnits.DigIn.Instance
        }.Concat(DEFAULT_ACTIONS).ToList();

        public override IReadOnlyDictionary<SupplyType, int> SupplyLimits { get; } = new Dictionary<SupplyType, int>()
        {
            { SupplyTypes.Bullets.Instance, 14 },
            { SupplyTypes.Diesel.Instance, 80 },
            { SupplyTypes.Rations.Instance, 60 }
        };

        private TransportHalfTrack() : base("transport_half_track") { }
        public static TransportHalfTrack Instance { get; } = new TransportHalfTrack();
    }
}
