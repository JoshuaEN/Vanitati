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

        public ActionInfo(ActionType type, Location source, Location target = null)
        {
            Type = type;
            Source = source;
            Target = target;
        }
    }
}
