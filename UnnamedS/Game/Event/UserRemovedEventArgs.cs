using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UserRemovedEventArgs : EventArgs
    {
        public int UserID { get; }

        public UserRemovedEventArgs(int userID)
        {
            UserID = userID;
        }
    }
}
