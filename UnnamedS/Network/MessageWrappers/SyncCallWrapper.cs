using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SyncCallWrapper : CallMessageWrapper
    {
        public override bool RequiresHost { get; } = false;

        public int SyncID { get; }
        public SyncCallWrapper(int syncID)
        {
            SyncID = syncID;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.Sync(SyncID);
        }
    }
}
