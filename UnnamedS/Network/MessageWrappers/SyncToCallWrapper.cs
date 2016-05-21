using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class SyncToCallWrapper : CallMessageWrapper
    {
        public override bool RequiresHost { get; } = true;

        public int SyncID { get; }
        public BattleGameState.Fields Fields { get; }
        public GameLogic.Fields LogicFields { get; }

        public SyncToCallWrapper(int syncID, BattleGameState.Fields fields, GameLogic.Fields logicFields)
        {
            Contract.Requires<ArgumentNullException>(null != fields);
            Contract.Requires<ArgumentNullException>(null != logicFields);

            SyncID = syncID;
            Fields = fields;
            LogicFields = logicFields;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.Sync(SyncID, Fields, LogicFields);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Fields);
            Contract.Invariant(null != LogicFields);
        }
    }
}
