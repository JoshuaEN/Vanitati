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
        public abstract bool CanPerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile = null);

        [Pure]
        public abstract IReadOnlyList<StateChange> PerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile = null);

        public enum ActionTarget { Self, Other };

        [Flags]
        public enum ActionTriggers { TurnStart = 1, TurnEnd = 2, AttributeChange = 4, UnitCreated = 8, UnitDestroyed = 16, None = 0 }

        protected ActionType(string key, ActionTarget targets, ActionTriggers triggers, bool availableDuringTurn = false ) : base("action_" + key)
        {
            Contract.Requires<ArgumentException>(availableDuringTurn == true || triggers != ActionTriggers.None, "An action cannot both be unavailable during the turn and have no triggers");

            _targets = targets;
            _availableDuringTurn = availableDuringTurn;
            _triggers = triggers;
        }

        protected ActionType(string key, ActionTriggers triggers, bool availableDuringTurn = false) : this(key, ActionTarget.Other, triggers, availableDuringTurn) { }
        protected ActionType(string key, ActionTarget targets) : this(key, targets, ActionTriggers.None, true) { }
        protected ActionType(string key) : this(key, ActionTarget.Other) { }


        public static IReadOnlyDictionary<string, ActionType> TYPES { get; }

        static ActionType()
        {
            TYPES = BuildTypeListing<ActionType>("UnnamedStrategyGame.Game.ActionTypes");
        }
    }

    [ContractClassFor(typeof(ActionType))]
    internal abstract class ContractClassForActionType : ActionType
    {
        [Pure]
        public override bool CanPerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile = null)
        {
            Contract.Requires<ArgumentNullException>(state != null);
            Contract.Requires<ArgumentNullException>(sourceTile != null);
            Contract.Requires<ArgumentNullException>(targetTile != null || Targets == ActionTarget.Self);
            Contract.Requires<ArgumentException>(targetTile == null && Targets == ActionTarget.Self, "Actions which target themselves cannot have a target tile");

            return false;
        }

        [Pure]
        public override IReadOnlyList<StateChange> PerformOn(BattleGameState state, Action.ActionContext context, Tile sourceTile, Tile targetTile = null)
        {
            Contract.Requires<ArgumentNullException>(state != null);
            Contract.Requires<ArgumentNullException>(sourceTile != null);
            Contract.Requires<ArgumentNullException>(targetTile != null || Targets == ActionTarget.Self);
            Contract.Requires<ArgumentException>(targetTile == null && Targets == ActionTarget.Self, "Actions which target themselves cannot have a target tile");
            Contract.Requires<NotSupportedException>(CanPerformOn(state, context, sourceTile, targetTile));
            Contract.Ensures(Contract.Result<IReadOnlyList<StateChange>>() != null);

            return null;
        }

        private ContractClassForActionType() : base(null) { }
    }
}
