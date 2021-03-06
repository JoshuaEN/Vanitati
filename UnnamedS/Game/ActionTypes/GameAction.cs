﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class GameAction : ActionType
    {
        public abstract ActionTriggers Triggers { get; }

        public sealed override bool CanUserTrigger { get; } = false;

        public sealed override Category ActionCategory
        {
            get { return Category.Commander; }
        }

        protected GameAction(string key) : base("game_" + key) { }

        [Flags]
        public enum ActionTriggers
        {
            DirectlyByGameLogic = 1,
            OnAnyTurnStart = 2,
            OnAnyTurnEnd = 4,
            OnRoundStart = 8,
            OnRoundEnd = 16,
            None = 0
        }
    }
}
