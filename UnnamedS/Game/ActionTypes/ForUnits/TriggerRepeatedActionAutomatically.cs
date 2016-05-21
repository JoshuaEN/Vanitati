using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class TriggerRepeatedActionAutomatically : UnitTargetOtherAction
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.OnTurnEnd;

        private TriggerRepeatedActionAutomatically() : base("trigger_repeated_action_automatically") { }
        public static TriggerRepeatedActionAutomatically Instance { get; } = new TriggerRepeatedActionAutomatically();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            return true;
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            return new List<Modifier>(0);
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            if (sourceTile.Unit.RepeatedAction == null || sourceTile.Unit.RepeatedAction.Type == NullUnitAction.Instance)
                return new List<StateChange>(0);

            var action = sourceTile.Unit.RepeatedAction;

            if(action.Type.CanPerformOn(state, action.Context) == false)
            {
                return new List<StateChange>()
                {
                    new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                    {
                        { "RepeatedAction", NullUnitAction.ActionInfoInstance }
                    }, sourceTile.Unit.Location)
                };
            }

            return action.Type.PerformOn(state, action.Context);
        }
    }
}
