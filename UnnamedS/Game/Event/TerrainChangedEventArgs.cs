using System;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TerrainChangedEventArgs : System.EventArgs, ITurnAwareEvent
    {
        public int TurnID { get; }
        public StateChanges.TerrainStateChange ChangeInfo { get; }

        public TerrainChangedEventArgs(int turnID, StateChanges.TerrainStateChange changeInfo)
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