﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForCommanders
{
    [ContractClass(typeof(ContractClassForCommanderTargetOtherAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CommanderTargetOtherAction : CommanderAction
    {
        public sealed override TargetCategory ActionTargetCategory
        {
            get { return TargetCategory.Other; }
        }

        protected CommanderTargetOtherAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetOtherContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceCommander);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetOtherContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceCommander);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetOtherContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceCommander);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander);
    }


    [ContractClassFor(typeof(CommanderTargetOtherAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForCommanderTargetOtherAction : CommanderTargetOtherAction
    {
        private ContractClassForCommanderTargetOtherAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetOtherContext context, Commander sourceCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
