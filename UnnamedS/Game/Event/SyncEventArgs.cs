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
        public GameLogic.Fields LogicFields { get; }

        public SyncEventArgs(int syncID, BattleGameState.Fields fields, GameLogic.Fields logicFields)
        {
            Contract.Requires<ArgumentNullException>(null != fields);
            Contract.Requires<ArgumentNullException>(null != logicFields);

            SyncID = syncID;
            Fields = fields;
            LogicFields = logicFields;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Fields);
            Contract.Invariant(null != LogicFields);
        }
    }
}
