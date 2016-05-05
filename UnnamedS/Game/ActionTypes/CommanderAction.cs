using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.ActionTypes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CommanderAction : ActionType
    {
        public abstract ActionTriggers Triggers { get; }

        public sealed override bool CanUserTrigger
        {
            get { return Triggers.HasFlag(ActionTriggers.ManuallyByUser); }
        }

        public sealed override Category ActionCategory
        {
            get { return Category.Commander; }
        }

        protected CommanderAction(string key) : base("commander_" + key) { }

        [Flags]
        public enum ActionTriggers
        {
            ManuallyByUser = 1,
            DirectlyByGameLogic = 2,
            TurnStart = 4,
            TurnEnd = 8,
            None = 0
        }
    }
}
