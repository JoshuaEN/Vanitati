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
    public abstract class TargetContextBase<TSource, TTarget, TTrigger> where TSource:Context where TTarget:Context
    {
        public TSource Source { get; }
        public TTarget Target { get; }
        public Load Loaded { get; }

        public bool LoadedSource { get { return Loaded.HasFlag(Load.Source); } }
        public bool LoadedTarget { get { return Loaded.HasFlag(Load.Target); } }

        public TTrigger Trigger { get; }

        protected TargetContextBase(IReadOnlyBattleGameState state, ActionContext context, Load load)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);

            Loaded = load;

            TSource source;
            TTarget target;
            context.ConvertToSpecificContext(load, out source, out target);

            Source = source;
            Target = target;

            if (context.Trigger is TTrigger == false)
                throw new ArgumentException($"Expected Trigger of type {typeof(TTrigger)}, got {context.Trigger.GetType()}");

            Trigger = (TTrigger)(object)context.Trigger;
                
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Source || Loaded.HasFlag(Load.Source) == false);
            Contract.Invariant(null != Target || Loaded.HasFlag(Load.Target) == false);
        }

    }

    public static class TargetContextBase
    {
        [Flags]
        public enum Load { Source = 1, Target = 2 }
    }
}
