using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForCommanders
{
    public sealed class EndTurn : CommanderTargetOtherAction
    {
        private EndTurn() : base("end_turn") { }
        public static EndTurn Instance { get; } = new EndTurn();

        public override ActionTriggers Triggers { get; } = ActionTriggers.ManuallyByUser;

        public override bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            if (sourceCommander.CommanderID != state.CurrentCommander?.CommanderID)
                return false;

            return true;
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            return new List<StateChange>()
            {
                new StateChanges.TurnEnd(sourceCommander.CommanderID)
            };
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            return new List<Modifier>(0);
        }
    }
}
