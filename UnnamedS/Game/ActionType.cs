using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    [ContractClass(typeof(ContractClassForActionType))]
    public abstract class ActionType : BaseType
    {
        private readonly ActionTarget _targets;
        public ActionTarget Targets { get { return _targets; } }

        private readonly ActionTriggers _triggers;
        public ActionTriggers Triggers { get { return _triggers; } }

        public bool TriggerOnTurnStart { get { return Triggers.HasFlag(ActionTriggers.TurnStart); } }
        public bool TriggerOnTurnEnd { get { return Triggers.HasFlag(ActionTriggers.TurnEnd); } }
        public bool TriggerOnAttributeChange { get { return Triggers.HasFlag(ActionTriggers.AttributeChange); } }
        public bool TriggerOnUnitCreated { get { return Triggers.HasFlag(ActionTriggers.UnitCreated); } }
        public bool TriggerOnUnitDestroyed { get { return Triggers.HasFlag(ActionTriggers.UnitDestroyed); } }

        private readonly bool _availableDuringTurn;
        public bool AvailableDuringTurn { get { return _availableDuringTurn; } }

        [Pure]
        public virtual bool CanPerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile)
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
                switch (Targets)
                {
                    case ActionTarget.AnyOtherUnit:
                        return targetTile.Unit != null && IsTargetingSelf(sourceTile, targetTile) == false;
                    case ActionTarget.AnyUnit:
                        return targetTile.Unit != null;
                    case ActionTarget.Captureable:
                        return targetTile.Terrain.GetAttribute(TerrainType.CAPTURE_POINTS).GetValue<int>() > 0;
                    case ActionTarget.Empty:
                        return targetTile.Unit == null;
                    case ActionTarget.EnemyUnit:
                        // TODO Support allies
                        return targetTile.Unit != null && targetTile.Unit.GetAttribute(UnitType.PLAYER_ID).GetValue<int>() != context.PlayerID;
                    case ActionTarget.AllyUnit:
                        // TODO Support allies
                        return targetTile.Unit != null && targetTile.Unit.GetAttribute(UnitType.PLAYER_ID).GetValue<int>() == context.PlayerID;
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
        public abstract IReadOnlyList<StateChange> PerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile);

        public enum ActionTarget { Unset, Any, Self, AllyUnit, EnemyUnit, AnyOtherUnit, AnyUnit, Empty, Captureable };

        [Flags]
        public enum ActionTriggers { TurnStart = 1, TurnEnd = 2, AttributeChange = 4, UnitCreated = 8, UnitDestroyed = 16, None = 0 }

        protected ActionType(string key, ActionTarget targets, ActionTriggers triggers, bool availableDuringTurn = false ) : base("action_" + key)
        {
            Contract.Requires<ArgumentException>(availableDuringTurn == true || triggers != ActionTriggers.None, "An action cannot both be unavailable during the turn and have no triggers");

            _targets = targets;
            _availableDuringTurn = availableDuringTurn;
            _triggers = triggers;
        }

        protected ActionType(string key, ActionTriggers triggers, bool availableDuringTurn = false) : this(key, ActionTarget.Unset, triggers, availableDuringTurn) { }
        protected ActionType(string key, ActionTarget targets) : this(key, targets, ActionTriggers.None, true) { }
        protected ActionType(string key) : this(key, ActionTarget.Unset) { }


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

            var locA = sourceTile.Terrain.GetAttribute(TerrainType.LOCATION).GetValue<Location>();
            var locB = targetTile.Terrain.GetAttribute(TerrainType.LOCATION).GetValue<Location>();
            Contract.Assert(null != locA);
            Contract.Assert(null != locB);
            return locA.Equals(locB);
        }
    }

    [ContractClassFor(typeof(ActionType))]
    internal abstract class ContractClassForActionType : ActionType
    {
        [Pure]
        public override IReadOnlyList<StateChange> PerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile = null)
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

        private ContractClassForActionType() : base(null) { }
    }
}
