using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class TurnEndedEventArgs : EventArgs
    {
        public int CommanderID { get; }
        public TurnEndedEventArgs(int commanderID)
        {
            CommanderID = commanderID;
        }
    }
}
