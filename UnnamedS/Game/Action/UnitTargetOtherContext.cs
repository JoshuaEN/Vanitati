using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnnamedStrategyGame.Game.Action.TargetContextBase;

namespace UnnamedStrategyGame.Game.Action
{
    public class UnitTargetOtherContext : TileTargetOtherContext<UnitContext, ActionTypes.UnitAction.ActionTriggers>
    {
        public UnitTargetOtherContext(IReadOnlyBattleGameState state, ActionContext context) : base(state, context) { }
    }
}
