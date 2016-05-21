using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TurnChangedEventArgs : EventArgs, ITurnAwareEvent, IActionTriggeredEvent
    {
        public int TurnID { get; }
        public ActionIdentifyingInfo ActionIdentifyingInfo { get; }
        public StateChanges.TurnChanged ChangeInfo { get; }

        public TurnChangedEventArgs(int turnID, ActionIdentifyingInfo actionIdentifyingInfo, StateChanges.TurnChanged changeInfo)
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
