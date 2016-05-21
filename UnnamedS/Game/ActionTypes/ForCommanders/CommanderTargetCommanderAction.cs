using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForCommanders
{
    [ContractClass(typeof(ContractClassForCommanderTargetCommanderAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CommanderTargetCommanderAction : CommanderAction
    {
        public sealed override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(Action.Wrapper.CommanderID)
        };

        protected CommanderTargetCommanderAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetCommanderContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetCommander);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetCommanderContext context, Commander sourceCommander, Commander targetCommander);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetCommanderContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetCommander);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetCommanderContext context, Commander sourceCommander, Commander targetCommander);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetCommanderContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetCommander);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetCommanderContext context, Commander sourceCommander, Commander targetCommander);
    }


    [ContractClassFor(typeof(CommanderTargetCommanderAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForCommanderTargetCommanderAction : CommanderTargetCommanderAction
    {
        private ContractClassForCommanderTargetCommanderAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetCommanderContext context, Commander sourceCommander, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetCommander);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetCommanderContext context, Commander sourceCommander, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetCommanderContext context, Commander sourceCommander, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
