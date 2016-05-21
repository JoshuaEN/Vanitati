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
    public abstract class TargetContextBase<TSource, TTrigger> where TSource : SourceContext
    {
        public int? TriggeredByCommanderID { get; }

        public TSource Source { get; }
        public Load Loaded { get; }

        public bool LoadedSource { get { return Loaded.HasFlag(Load.Source); } }
        public bool LoadedTarget { get { return Loaded.HasFlag(Load.Target); } }

        public TTrigger Trigger { get; }

        protected TargetContextBase(IReadOnlyBattleGameState state, ActionContext context, Load load)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);

            Loaded = load;
            TriggeredByCommanderID = context.TriggeredByCommanderID;

            TSource source = (context.Source as TSource);

            if (load.HasFlag(Load.Source))
            {
                if(source == null)
                    throw new ArgumentException($"Action context's source type is {context.Source.GetType()}, not {typeof(TSource)}");
            }

            Source = source;

            if (context.Trigger is TTrigger == false)
                throw new ArgumentException($"Expected Trigger of type {typeof(TTrigger)}, got {context.Trigger.GetType()}");

            Trigger = (TTrigger)(object)context.Trigger;

        }

        protected static TargetContext CastTargetContext(Context context)
        {
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Ensures(Contract.Result<TargetContext>() != null);


            var gContext = (TargetContext)context;

            if (gContext == null)
                throw new ArgumentException($"Expected target context to be of type GenericContext, not {context.GetType()}");

            return gContext;
        }

        protected static void CheckTargetContext(TargetContext context, int expectedLength)
        {
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentOutOfRangeException>(expectedLength >= 0);

            if (context.Values.Count != expectedLength)
                throw new ArgumentException($"Expected target values to be of length {expectedLength}, not {context.Values.Count}");

        }

        protected static TValueType LoadTargetContextValue<TValueType>(TargetContext context, Load load, int valueAt)
        {
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentException>(valueAt >= 0);

            if(context.Values.Count <= valueAt)
            {
                if (load.HasFlag(Load.Target))
                    throw new ArgumentException($"Context value listing is of length {context.Values.Count}, expected count of at least {valueAt + 1}");
                else
                    return default(TValueType);
            }

            TValueType outValue = (TValueType)context.Values[valueAt];

            if (load.HasFlag(Load.Target))
            {
                if (outValue == null)
                    throw new ArgumentException($"Action context's source type is {context.Values[valueAt].GetType()}, not {typeof(TValueType)}");
            }

            return outValue;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Source || Loaded.HasFlag(Load.Source) == false);
        }

    }

    public static class TargetContextBase
    {
        [Flags]
        public enum Load { None = 0, Source = 1, Target = 2 }
    }
}
