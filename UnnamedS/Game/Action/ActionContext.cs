using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class ActionContext
    {
        public int CommanderID { get; }
        public ActionType.ActionTriggers Trigger { get; }
        public bool ManuallyActivated { get { return Trigger == ActionType.ActionTriggers.None; } }

        public ActionContext(int commanderID, ActionType.ActionTriggers trigger)
        {
            CommanderID = commanderID;
            Trigger = trigger;
        }

        public ActionContext(int commanderID) : this(commanderID, ActionType.ActionTriggers.None)
        {

        }
    }
}
