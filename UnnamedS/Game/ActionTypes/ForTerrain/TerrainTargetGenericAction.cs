using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    [ContractClass(typeof(ContractClassForTerrainTargetGenericAction<>))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TerrainTargetGenericAction<T> : TerrainAction
    {
        public sealed override TargetCategory ActionTargetCategory
        {
            get { return TargetCategory.Generic; }
        }

        public TerrainTargetGenericAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetGenericContext<T>(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetValue);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile, T targetValue);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetGenericContext<T>(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetValue);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile, T targetValue);

        public sealed override IReadOnlyList<object> AvailableOptions(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetGenericContext<T>(state, context, TargetContextBase.Load.Source);
            return AvailableOptions(state, convertedContext, convertedContext.SourceTile).Cast<object>().ToList();
        }
        public abstract IReadOnlyList<T> AvailableOptions(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetGenericContext<T>(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetValue);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile, T targetValue);
    }

    [ContractClassFor(typeof(TerrainTargetGenericAction<>))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForTerrainTargetGenericAction<T> : TerrainTargetGenericAction<T>
    {
        private ContractClassForTerrainTargetGenericAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != targetValue);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetValue);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<T> AvailableOptions(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetGenericContext<T> context, Tile sourceTile, T targetValue)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetValue);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
