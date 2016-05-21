using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForGame
{
    [ContractClass(typeof(ContractClassForGameTargetOtherAction))]
    public abstract class GameTargetOtherAction : GameAction
    {
        protected GameTargetOtherAction(string key) : base(key) { }

        public sealed override Type[] TargetValueTypes { get; } = new Type[0];


        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            return true;
        }

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new GameTargetOtherContext(state, context);
            return PerformOn(state, convertedContext);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, GameTargetOtherContext context);

        public sealed override System.Collections.IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            throw new NotSupportedException();
        }

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            throw new NotSupportedException();
        }
    }

    [ContractClassFor(typeof(GameTargetOtherAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForGameTargetOtherAction : GameTargetOtherAction
    {
        private ContractClassForGameTargetOtherAction() : base(null) { }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, GameTargetOtherContext context)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

    }
}
