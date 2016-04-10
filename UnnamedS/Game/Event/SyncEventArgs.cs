using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class SyncEventArgs : EventArgs
    {
        public int SyncID { get; }
        public BattleGameState.Fields Fields { get; }

        public SyncEventArgs(int syncID, BattleGameState.Fields fields)
        {
            SyncID = syncID;
            Fields = fields;
        }
    }
}
