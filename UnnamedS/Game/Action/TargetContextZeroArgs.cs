using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnnamedStrategyGame.Game.Action.TargetContextBase;

namespace UnnamedStrategyGame.Game.Action
{
    public class TargetContextZeroArgs<TSource, TTrigger> : TargetContextBase<TSource, TTrigger> where TSource:SourceContext
    {
        public TargetContextZeroArgs(IReadOnlyBattleGameState state, ActionContext context, Load load) : base(state, context, load) { }
    }
}
