using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class TerrainTargetOtherContext : TileTargetOtherContext<TerrainContext, ActionTypes.TerrainAction.ActionTriggers>
    {
        public TerrainTargetOtherContext(IReadOnlyBattleGameState state, ActionContext context) : base(state, context) { }
    }
}
