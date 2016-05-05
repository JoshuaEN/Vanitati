using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SyncEventArgs : EventArgs
    {
        public int SyncID { get; }
        public BattleGameState.Fields Fields { get; }

        public SyncEventArgs(int syncID, BattleGameState.Fields fields)
        {
            Contract.Requires<ArgumentNullException>(null != fields);

            SyncID = syncID;
            Fields = fields;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Fields);
        }
    }
}
