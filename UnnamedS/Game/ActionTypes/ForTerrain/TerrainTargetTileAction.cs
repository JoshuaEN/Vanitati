using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForTerrain
{
    [ContractClass(typeof(ContractClassForTerrainTargetTileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class TerrainTargetTileAction : TerrainAction
    {
        public sealed override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(Location)
        };

        protected TerrainTargetTileAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetTileContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetTile);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile, Tile targetTile);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetTileContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetTile);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile, Tile targetTile);

        public sealed override System.Collections.IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetTileContext(state, context, TargetContextBase.Load.Source);
            return ValidTargets(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new TerrainTargetTileContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetTile);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile, Tile targetTile);
    }


    [ContractClassFor(typeof(TerrainTargetTileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForTerrainTargetTileAction : TerrainTargetTileAction
    {
        private ContractClassForTerrainTargetTileAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetTile);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<Location, ActionChain>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, TerrainTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
