using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public interface IActionTriggeredEvent
    {
        ActionIdentifyingInfo ActionIdentifyingInfo { get; }
    }

    public class ActionIdentifyingInfo
    {
        public int ActionID { get; }
        public int UserID { get; }

        public ActionIdentifyingInfo(int actionID, int userID)
        {
            ActionID = actionID;
            UserID = userID;
        }
    }
}
