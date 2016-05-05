using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    [ContractClass(typeof(ContractClassForUnitTargetOtherAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UnitTargetOtherAction : UnitAction
    {
        public sealed override TargetCategory ActionTargetCategory
        {
            get { return TargetCategory.Other; }
        }

        protected UnitTargetOtherAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetOtherContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetOtherContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetOtherContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile);
    }


    [ContractClassFor(typeof(UnitTargetOtherAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForUnitTargetOtherAction : UnitTargetOtherAction
    {
        private ContractClassForUnitTargetOtherAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetOtherContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
