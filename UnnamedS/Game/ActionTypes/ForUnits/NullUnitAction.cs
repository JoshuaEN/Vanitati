using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Game.ActionTypes.ForUnits
{
    public sealed class NullUnitAction : UnitAction
    {
        public override TargetCategory ActionTargetCategory { get; } = TargetCategory.Other;

        public override ActionTriggers Triggers { get; } = ActionTriggers.None;

        private NullUnitAction() : base("null_unit_action") { }
        public static NullUnitAction Instance { get; } = new NullUnitAction();
        public static ActionInfo ActionInfoInstance { get; } = new ActionInfo(Instance, new ActionContext(null, ActionTriggers.None, new UnitContext(new Location(0, 0)), new OtherContext()));

        public override bool CanPerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            return false;
        }

        public override IReadOnlyList<Modifier> Modifiers(IReadOnlyBattleGameState state, ActionContext context)
        {
            return new List<Modifier>(0);
        }

        public override IReadOnlyList<StateChange> PerformOn(IReadOnlyBattleGameState state, ActionContext context)
        {
            return new List<StateChange>(0);
        }

        public override IReadOnlyDictionary<Location, ActionChain> ActionableLocations(IReadOnlyBattleGameState state, ActionContext context)
        {
            return new Dictionary<Location, ActionChain>(0);
        }
    }
}
