using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class UserAssignedToCommanderEventArgs : EventArgs
    {
        public int? UserID { get; }
        public int CommanderID { get; }

        public UserAssignedToCommanderEventArgs(int? userID, int commanderID)
        {
            UserID = userID;
            CommanderID = commanderID;
        }
    }
}
