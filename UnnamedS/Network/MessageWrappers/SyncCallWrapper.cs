using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class SyncCallWrapper : CallMessageWrapper
    {
        public int SyncID { get; }
        public SyncCallWrapper(int syncID) : base()
        {
            SyncID = syncID;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.Sync(SyncID);
        }
    }
}
