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
        public override ActionTriggers Triggers { get; } = ActionTriggers.OnTurnStart | ActionTriggers.OnTurnEnd;

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

            var unitAction = (action.Type as UnitAction);

            if (unitAction == null)
                throw new ArgumentException($"Cannot repeat non-unit action of {action.Type}");

            if (unitAction.RepeatOn == RepeatFlags.None)
                throw new ArgumentException($"Repeat On flag must be set");

            if (context.Trigger != ActionTriggers.OnTurnStart && context.Trigger != ActionTriggers.OnTurnEnd)
                throw new ArgumentException($"Invalid Trigger of {context.Trigger}");

            if (
                (context.Trigger == ActionTriggers.OnTurnStart && unitAction.RepeatOn.HasFlag(RepeatFlags.OnTurnStart) == false) ||
                (context.Trigger == ActionTriggers.OnTurnEnd && unitAction.RepeatOn.HasFlag(RepeatFlags.OnTurnEnd) == false))
                return new List<StateChange>(0);

            if (action.Type.CanPerformOn(state, action.Context) == false)
            {
                return new List<StateChange>()
                {
                    GetClearRepeatedActionChange(sourceTile)
                };
            }

            return action.Type.PerformOn(state, action.Context);
        }
    }
}
