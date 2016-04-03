using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class ActionContext
    {
        public int PlayerID { get; }
        public ActionType.ActionTriggers Trigger { get; }
        public bool ManuallyActivated { get { return Trigger == ActionType.ActionTriggers.None; } }

        public ActionContext(int playerID, ActionType.ActionTriggers trigger)
        {
            PlayerID = playerID;
            Trigger = trigger;
        }

        public ActionContext(int playerID) : this(playerID, ActionType.ActionTriggers.None)
        {

        }
    }
}
