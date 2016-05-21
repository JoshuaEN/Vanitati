using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    [ContractClass(typeof(ContractClassForUnitTargetCommanderAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UnitTargetCommanderAction : UnitAction
    {
        public sealed override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(Action.Wrapper.CommanderID)
        };

        protected UnitTargetCommanderAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetCommanderContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetCommander);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile, Commander targetCommander);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetCommanderContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetCommander);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile, Commander targetCommander);

        public sealed override System.Collections.IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetCommanderContext(state, context);
            return ValidTargets(state, convertedContext, convertedContext.SourceTile);
        }
        public abstract IReadOnlyList<Action.Wrapper.CommanderID> ValidTargets(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new UnitTargetCommanderContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceTile, convertedContext.TargetCommander);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile, Commander targetCommander);

        protected sealed override bool RangeBasedValidTargetCanPerform(IReadOnlyBattleGameState state, UnitTargetTileContext context, Tile sourceTile, Tile targetTile)
        {
            throw new NotSupportedException();
        }
    }


    [ContractClassFor(typeof(UnitTargetCommanderAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForUnitTargetCommanderAction : UnitTargetCommanderAction
    {
        private ContractClassForUnitTargetCommanderAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetCommander);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Action.Wrapper.CommanderID> ValidTargets(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Ensures(Contract.Result<IReadOnlyList<Action.Wrapper.CommanderID>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, UnitTargetCommanderContext context, Tile sourceTile, Commander targetCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != sourceTile.Unit);
            Contract.Requires<ArgumentNullException>(null != targetCommander);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
