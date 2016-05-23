using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class VictoryConditionAchievedEventArgs : EventArgs, IActionTriggeredEvent
    {
        public ActionIdentifyingInfo ActionIdentifyingInfo { get; }
        public StateChanges.VictoryConditionAchieved ChangeInfo { get; }

        public VictoryConditionAchievedEventArgs(ActionIdentifyingInfo actionIdentifyingInfo, StateChanges.VictoryConditionAchieved changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);

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
