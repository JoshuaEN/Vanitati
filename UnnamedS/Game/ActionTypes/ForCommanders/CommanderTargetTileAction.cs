using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForCommanders
{
    [ContractClass(typeof(ContractClassForCommanderTargetTileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CommanderTargetTileAction : CommanderAction
    {
        public sealed override Type[] TargetValueTypes { get; } = new Type[]
        {
            typeof(Location)
        };

        protected CommanderTargetTileAction(string key) : base(key) { }

        public sealed override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetTileContext(state, context);
            return CanPerformOn(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetTile);
        }
        public abstract bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander, Tile targetTile);

        public sealed override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetTileContext(state, context);
            return PerformOn(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetTile);
        }
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander, Tile targetTile);

        public sealed override System.Collections.IEnumerable ValidTargets(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetTileContext(state, context, TargetContextBase.Load.Source);
            return ValidTargets(state, convertedContext, convertedContext.SourceCommander);
        }
        public abstract IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander);

        public sealed override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            var convertedContext = new CommanderTargetTileContext(state, context);
            return Modifiers(state, convertedContext, convertedContext.SourceCommander, convertedContext.TargetTile);
        }
        public abstract IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander, Tile targetTile);
    }


    [ContractClassFor(typeof(CommanderTargetTileAction))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForCommanderTargetTileAction : CommanderTargetTileAction
    {
        private ContractClassForCommanderTargetTileAction() : base(null) { }

        public override bool CanPerformOn(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetTile);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyDictionary<Location, ActionChain> ValidTargets(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<Location, ActionChain>>() != null);

            throw new NotSupportedException();
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, CommanderTargetTileContext context, Commander sourceCommander, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceCommander);
            Contract.Requires<ArgumentNullException>(null != targetTile);
            Contract.Ensures(Contract.Result<IReadOnlyList<Modifier>>() != null);

            throw new NotSupportedException();
        }
    }
}
