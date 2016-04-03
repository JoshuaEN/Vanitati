using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class GameStartEventArgs : EventArgs
    {
        public StateChanges.GameStarted ChangeInfo { get; }

        public GameStartEventArgs(StateChanges.GameStarted changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
