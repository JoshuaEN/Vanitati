using System;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UnitChangedEventArgs : System.EventArgs, ITurnAwareEvent, IActionTriggeredEvent
    {
        public int TurnID { get; }
        public ActionIdentifyingInfo ActionIdentifyingInfo { get; }
        public StateChanges.UnitStateChange ChangeInfo { get; }

        public UnitChangedEventArgs(int turnID, ActionIdentifyingInfo actionIdentifyingInfo, StateChanges.UnitStateChange changeInfo)
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