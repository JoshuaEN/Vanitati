using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class ActionContext
    {
        public ActionType.ActionTriggers Trigger { get; }
        public bool ManuallyActivated { get { return Trigger == ActionType.ActionTriggers.None; } }

        public ActionContext(ActionType.ActionTriggers trigger)
        {
            Trigger = trigger;
        }

        public ActionContext()
        {
            Trigger = ActionType.ActionTriggers.None;
        }
    }
}
