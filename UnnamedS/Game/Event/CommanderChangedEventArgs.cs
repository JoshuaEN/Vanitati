using System;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CommanderChangedEventArgs : EventArgs, ITurnAwareEvent
    {
        public int TurnID { get; }
        public StateChanges.CommanderStateChange ChangeInfo { get; }

        public CommanderChangedEventArgs(int turnID, StateChanges.CommanderStateChange changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);

            TurnID = turnID;
            ChangeInfo = changeInfo;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != ChangeInfo);
        }
    }
}