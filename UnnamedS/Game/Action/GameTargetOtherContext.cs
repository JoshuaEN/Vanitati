using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class GameTargetOtherContext : TargetContextZeroArgs<GameContext, ActionTypes.GameAction.ActionTriggers>
    {
        public GameTargetOtherContext(IReadOnlyBattleGameState state, ActionContext context) : base(state, context, TargetContextBase.Load.None)
        {

        }
    }
}
