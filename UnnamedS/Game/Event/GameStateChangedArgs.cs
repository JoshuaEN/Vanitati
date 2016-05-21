using System;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameStateChangedArgs : System.EventArgs, ITurnAwareEvent, IActionTriggeredEvent
    {
        public int TurnID { get; }
        public ActionIdentifyingInfo ActionIdentifyingInfo { get; }
        public StateChanges.GameStateChange ChangeInfo { get; }

        public GameStateChangedArgs(int turnID, ActionIdentifyingInfo actionIdentifyingInfo, StateChanges.GameStateChange changeInfo)
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