using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UserAssignedToCommanderEventArgs : EventArgs
    {
        public int? UserID { get; }
        public int CommanderID { get; }
        public int PerformedByUserID { get; }
        public bool WasHost { get; }

        public UserAssignedToCommanderEventArgs(int? userID, int commanderID, int performedByUserID, bool wasHost)
        {
            UserID = userID;
            CommanderID = commanderID;
            PerformedByUserID = performedByUserID;
            WasHost = wasHost;
        }
    }
}
