using System;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UnitChangedEventArgs : System.EventArgs, ITurnAwareEvent
    {
        public int TurnID { get; }
        public StateChanges.UnitStateChange ChangeInfo { get; }

        public UnitChangedEventArgs(int turnID, StateChanges.UnitStateChange changeInfo)
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