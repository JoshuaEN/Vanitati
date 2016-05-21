using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnnamedStrategyGame.Game.Action.TargetContextBase;

namespace UnnamedStrategyGame.Game.Action
{
    public class TargetContextTwoArgs<TSource, TFirstTargetType, TSecondTargetType, TTrigger> : TargetContextBase<TSource, TTrigger> where TSource : SourceContext
    {
        public TFirstTargetType FirstTargetValue { get; }
        public TSecondTargetType SecondTargetValue { get; }

        public TargetContextTwoArgs(IReadOnlyBattleGameState state, ActionContext context, Load load) : base(state, context, load)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);

            var targetContext = CastTargetContext(context.Target);

            if (load.HasFlag(Load.Target))
            {
                CheckTargetContext(targetContext, 2);
            }

            FirstTargetValue = LoadTargetContextValue<TFirstTargetType>(targetContext, load, 0);
            SecondTargetValue = LoadTargetContextValue<TSecondTargetType>(targetContext, load, 1);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != FirstTargetValue || Loaded.HasFlag(Load.Target) == false);
            Contract.Invariant(null != SecondTargetValue || Loaded.HasFlag(Load.Target) == false);
        }
    }
}
