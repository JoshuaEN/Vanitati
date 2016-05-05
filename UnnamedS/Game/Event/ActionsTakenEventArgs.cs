using System;
using System.Collections.Generic;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ActionsTakenEventArgs : EventArgs
    {
        public List<ActionInfo> Actions { get; }
        public bool External { get; }

        public ActionsTakenEventArgs(List<ActionInfo> actions, bool external)
        {
            Actions = actions;
            External = external;
        }
    }
}