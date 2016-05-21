using System;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CommanderChangedEventArgs : EventArgs, ITurnAwareEvent, IActionTriggeredEvent
    {
        public int TurnID { get; }
        public ActionIdentifyingInfo ActionIdentifyingInfo { get; }
        public StateChanges.CommanderStateChange ChangeInfo { get; }

        public CommanderChangedEventArgs(int turnID, ActionIdentifyingInfo actionIdentifyingInfo, StateChanges.CommanderStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);

            TurnID = turnID;
            ActionIdentifyingInfo = actionIdentifyingInfo;
            ChangeInfo = changeInfo;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != ChangeInfo);
        }
    }
}