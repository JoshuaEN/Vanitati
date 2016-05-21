using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    [ContractClass(typeof(ContractClassForUnitTargetTileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UnitTargetTileAction : UnitAction
    {
        public sealed override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(Location)
        };

        protected UnitTargetTileAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetTileContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetTile);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetTileContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetTile);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile);

        public sealed override System.Collections.IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetTileContext(state, context, TargetContextBase.Load.Source);
            return ValidTargets(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetTileContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetTile);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile);

    }



    [ContractClassFor(typeof(UnitTargetTileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForUnitTargetTileAction : UnitTargetTileAction
    {
        private ContractClassForUnitTargetTileAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetTile);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<Location, ActionChain>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
