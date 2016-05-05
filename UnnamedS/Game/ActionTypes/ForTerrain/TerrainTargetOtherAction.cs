using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    [ContractClass(typeof(ContractClassForTerrainTargetOtherAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TerrainTargetOtherAction : TerrainAction
    {
        public sealed override TargetCategory ActionTargetCategory
        {
            get { return TargetCategory.Other; }
        }

        protected TerrainTargetOtherAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetOtherContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetOtherContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetOtherContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile);
    }


    [ContractClassFor(typeof(TerrainTargetOtherAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForTerrainTargetOtherAction : TerrainTargetOtherAction
    {
        private ContractClassForTerrainTargetOtherAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetOtherContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
