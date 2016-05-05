using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    [ContractClass(typeof(ContractClassForTerrainTargetCommanderAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TerrainTargetCommanderAction : TerrainAction
    {
        public sealed override TargetCategory ActionTargetCategory
        {
            get { return TargetCategory.Commander; }
        }

        protected TerrainTargetCommanderAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetCommanderContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetCommander);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetCommanderContext context, Tile sourceTile, Commander targetCommander);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetCommanderContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetCommander);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetCommanderContext context, Tile sourceTile, Commander targetCommander);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetCommanderContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetCommander);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetCommanderContext context, Tile sourceTile, Commander targetCommander);
    }


    [ContractClassFor(typeof(TerrainTargetCommanderAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForTerrainTargetCommanderAction : TerrainTargetCommanderAction
    {
        private ContractClassForTerrainTargetCommanderAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetCommanderContext context, Tile sourceTile, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetCommander);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetCommanderContext context, Tile sourceTile, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetCommanderContext context, Tile sourceTile, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
