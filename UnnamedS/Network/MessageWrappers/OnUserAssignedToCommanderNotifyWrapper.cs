using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnUserAssignedToCommanderNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.UserAssignedToCommanderEventArgs Args { get; }

        public OnUserAssignedToCommanderNotifyWrapper(Game.Event.UserAssignedToCommanderEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnUserAssignedToCommander(this, Args);
        }
    }
}
