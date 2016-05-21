using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForCommanders
{
    [ContractClass(typeof(ContractClassForCommanderTargetGenericAction<>))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CommanderTargetGenericAction<T> : CommanderAction
    {
        public sealed override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(T)
        };

        public CommanderTargetGenericAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetGenericContext<T>(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetValue);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander, T targetValue);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetGenericContext<T>(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetValue);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander, T targetValue);

        public sealed override System.Collections.IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetGenericContext<T>(state, context, TargetContextBase.Load.Source);
            return ValidTargets(state, convertedContext, convertedContext.SourceCommander);
        }
        public abstract IReadOnlyList<T> ValidTargets(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetGenericContext<T>(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetValue);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander, T targetValue);
    }

    [ContractClassFor(typeof(CommanderTargetGenericAction<>))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForCommanderTargetGenericAction<T> : CommanderTargetGenericAction<T>
    {
        private ContractClassForCommanderTargetGenericAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetValue);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetValue);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<T> ValidTargets(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetGenericContext<T> context, Commander sourceCommander, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetValue);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
