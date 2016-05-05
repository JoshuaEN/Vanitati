using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class ClearRepeatedActionManually : UnitTargetOtherAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        private ClearRepeatedActionManually() : base("clear_repeated_action_manually") { }
        public static ClearRepeatedActionManually Instance { get; } = new ClearRepeatedActionManually();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            return sourceTile.Unit.RepeatedAction.Type != NullUnitAction.Instance;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            if (sourceTile.Unit.RepeatedAction.Type == NullUnitAction.Instance)
                return new List<StateChange>(0);

            return new List<StateChange>()
            {
                new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                {
                    { "RepeatedAction", NullUnitAction.ActionInfoInstance }
                }, sourceTile.Location)
            };
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            return new List<Modifier>(0);
        }
    }
}
