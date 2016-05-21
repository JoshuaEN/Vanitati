using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnnamedStrategyGame.Game.Action.TargetContextBase;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TargetContextOneArg<TSource, TTargetType, TTrigger> : TargetContextBase<TSource, TTrigger> where TSource:SourceContext
    {
        public TTargetType TargetValue { get; }

        protected TargetContextOneArg(IReadOnlyBattleGameState state, ActionContext context, Load load) : base(state, context, load)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);

            var targetContext = CastTargetContext(context.Target);

            if (load.HasFlag(Load.Target))
            {
                CheckTargetContext(targetContext, 1);
            }

            TargetValue = LoadTargetContextValue<TTargetType>(targetContext, load, 0);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != TargetValue || Loaded.HasFlag(Load.Target) == false);
        }
    }
}
