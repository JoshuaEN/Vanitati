using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game
{
    [ContractClass(typeof(ContractClassForActionType))]
    public abstract class ActionType : BaseType
    {
        private readonly ActionTarget _targets;
        public ActionTarget Targets { get { return _targets; } }

        private readonly ActionTriggers _triggers;
        public ActionTriggers Triggers { get { return _triggers; } }

        private readonly bool _causesMovement;
        public bool CausesMovement { get { return _causesMovement; } }

        public bool TriggerOnTurnStart { get { return Triggers.HasFlag(ActionTriggers.TurnStart); } }
        public bool TriggerOnTurnEnd { get { return Triggers.HasFlag(ActionTriggers.TurnEnd); } }
        public bool TriggerOnAttributeChange { get { return Triggers.HasFlag(ActionTriggers.AttributeChange); } }
        public bool TriggerOnUnitCreated { get { return Triggers.HasFlag(ActionTriggers.UnitCreated); } }
        public bool TriggerOnUnitDestroyed { get { return Triggers.HasFlag(ActionTriggers.UnitDestroyed); } }

        private readonly bool _availableDuringTurn;
        public bool AvailableDuringTurn { get { return _availableDuringTurn; } }

        protected ActionType(string key, ActionTarget targets, ActionTriggers triggers, bool availableDuringTurn = false, bool causesMovement = false) : base("action_" + key)
        {
            Contract.Requires<ArgumentException>(availableDuringTurn == true || triggers != ActionTriggers.None, "An action cannot both be unavailable during the turn and have no triggers");

            _targets = targets;
            _availableDuringTurn = availableDuringTurn;
            _triggers = triggers;
            _causesMovement = causesMovement;
        }

        protected ActionType(string key, ActionTriggers triggers, bool availableDuringTurn = false, bool causesMovement = false) : this(key, ActionTarget.Unset, triggers, availableDuringTurn, causesMovement) { }
        protected ActionType(string key, ActionTarget targets, bool causesMovement = false) : this(key, targets, ActionTriggers.None, true, causesMovement) { }
        protected ActionType(string key) : this(key, ActionTarget.Unset) { }

        [Pure]
        public virtual bool CanPerformOn(IReadOnlyBattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetTile);

            if (ActionTarget.Unset == Targets)
            {
                throw new NotSupportedException("Either override CanPerformOn or set the ActionTarget");
            }

            if(ActionTarget.Any == Targets)
            {
                return true;
            }

            if (null != sourceTile.Unit)
            {
                if (sourceTile.Unit.UnitType.Actions.Contains(this) != true)
                {
                    return false;
                }

                switch (Targets)
                {
                    case ActionTarget.AnyOtherUnit:
                        return targetTile.Unit != null && IsTargetingSelf(sourceTile, targetTile) == false;
                    case ActionTarget.AnyUnit:
                        return targetTile.Unit != null;
                    case ActionTarget.Captureable:
                        return targetTile.Terrain.TerrainType.Captureable;
                    case ActionTarget.Empty:
                        return targetTile.Unit == null;
                    case ActionTarget.EnemyUnit:
                        // TODO Support allies
                        return targetTile.Unit != null && targetTile.Unit.CommanderID != context.CommanderID;
                    case ActionTarget.AllyUnit:
                        // TODO Support allies
                        return targetTile.Unit != null && targetTile.Unit.CommanderID == context.CommanderID;
                    case ActionTarget.Self:
                        return IsTargetingSelf(sourceTile, targetTile);
                    default:
                        throw new InvalidOperationException(string.Format("Unknown Action Target of {0}", Targets));
                }
            }
            else
            {
                // TODO Support constructing units with buildings
                return false;
            }


            throw new InvalidOperationException(string.Format("Unknown Action Target of {0}", Targets));
        }

        [Pure]
        public abstract IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile);

        [Pure]
        public abstract IReadOnlyDictionary<Location, Action.ActionChain> ActionableLocations(IReadOnlyBattleGameState state, Action.ActionContext context, Tile sourceTile);

        public enum ActionTarget { Unset, Any, Self, AllyUnit, EnemyUnit, AnyOtherUnit, AnyUnit, Empty, Captureable };

        [Flags]
        public enum ActionTriggers { TurnStart = 1, TurnEnd = 2, AttributeChange = 4, UnitCreated = 8, UnitDestroyed = 16, None = 0 }

        public static IReadOnlyDictionary<string, ActionType> TYPES { get; }

        static ActionType()
        {
            TYPES = BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes");
        }

        [Pure]
        private bool IsTargetingSelf(Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetTile);

            var locA = sourceTile.Terrain.Location;
            var locB = targetTile.Terrain.Location;
            Contract.Assert(null != locA);
            Contract.Assert(null != locB);
            return locA.Equals(locB);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(CausesMovement == false || this is ActionTypes.ICausesMovement);
        }
    }

    [ContractClassFor(typeof(ActionType))]
    internal abstract class ContractClassForActionType : ActionType
    {
        [Pure]
        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Requires<ArgumentNullException>(null != targetTile);
#if DEBUG
            Contract.Requires<NotSupportedException>(CanPerformOn(state, context, sourceTile, targetTile));
#endif
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            return null;
        }

        [Pure]
        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, Action.ActionContext context, Tile sourceTile)
        {
            Contract.Requires<ArgumentNullException>(null != state);
            Contract.Requires<ArgumentNullException>(null != context);
            Contract.Requires<ArgumentNullException>(null != sourceTile);
            Contract.Ensures(Contract.Result<IReadOnlyDictionary<Location, ActionChain>>() != null);

            return null;
        }

        private ContractClassForActionType() : base(null) { }
    }
}
