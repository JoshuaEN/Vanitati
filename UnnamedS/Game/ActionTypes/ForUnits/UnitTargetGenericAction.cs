using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    [ContractClass(typeof(ContractClassForUnitTargetGenericAction<>))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UnitTargetGenericAction<T> : UnitAction
    {
        public sealed override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(T)
        };

        public UnitTargetGenericAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetGenericContext<T>(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetValue);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile, T targetValue);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetGenericContext<T>(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetValue);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile, T targetValue);

        public sealed override System.Collections.IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetGenericContext<T>(state, context, TargetContextBase.Load.Source);
            return ValidTargets(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyList<T> ValidTargets(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetGenericContext<T>(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetValue);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile, T targetValue);

        protected sealed override bool RangeBasedValidTargetCanPerform(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            throw new NotSupportedException();
        }
    }

    [ContractClassFor(typeof(UnitTargetGenericAction<>))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForUnitTargetGenericAction<T> : UnitTargetGenericAction<T>
    {
        private ContractClassForUnitTargetGenericAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetValue);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetValue);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<T> ValidTargets(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetGenericContext<T> context, Tile sourceTile, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetValue);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
