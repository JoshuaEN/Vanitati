using System;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameStateChangedArgs : System.EventArgs, ITurnAwareEvent
    {
        public int TurnID { get; }
        public StateChanges.GameStateChange ChangeInfo { get; }

        public GameStateChangedArgs(int turnID, StateChanges.GameStateChange changeInfo)
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