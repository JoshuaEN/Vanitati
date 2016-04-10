using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public class ActionInfo
    {
        public ActionType Type { get; }
        public Location Source { get; }
        public Location Target { get; }
        public int CommanderID { get; }
        public ActionType.ActionTriggers Trigger { get; }

        public ActionInfo(int commanderID, ActionType type, Location source, Location target, ActionType.ActionTriggers trigger)
        {
            Type = type;
            Source = source;
            Target = target;
            CommanderID = commanderID;
            Trigger = trigger;
        }
    }
}
