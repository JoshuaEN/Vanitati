using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class ClearRepeatedActionAutomatically : UnitTargetGenericAction<ActionInfo>
    {
        public override ActionTriggers Triggers { get; } = ActionTriggers.OnActionPerformedByUser;
        private ClearRepeatedActionAutomatically() : base("clear_repeated_action_automatically") { }
        public static ClearRepeatedActionAutomatically Instance { get; } = new ClearRepeatedActionAutomatically();

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetGenericContext<ActionInfo> context, Tile sourceTile, ActionInfo targetValue)
        {
            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetGenericContext<ActionInfo> context, Tile sourceTile, ActionInfo targetValue)
        {

            if (sourceTile.Unit.RepeatedAction.Type == NullUnitAction.Instance)
                return new List<StateChange>(0);

            if(targetValue.Type == sourceTile.Unit.RepeatedAction.Type)
                return new List<StateChange>(0);

            return new List<StateChange>()
            {
                new StateChanges.UnitStateChange(sourceTile.Unit.UnitID, new Dictionary<string, object>()
                {
                    { "RepeatedAction", NullUnitAction.ActionInfoInstance }
                }, sourceTile.Location)
            };
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetGenericContext<ActionInfo> context, Tile sourceTile, ActionInfo targetValue)
        {
            return new List<Modifier>(0);
        }

        public override IReadOnlyList<ActionInfo> ValidTargets(IReadOnlyBattleGameState state, UnitTargetGenericContext<ActionInfo> context, Tile sourceTile)
        {
            return new List<ActionInfo>(0);
        }
    }
}
