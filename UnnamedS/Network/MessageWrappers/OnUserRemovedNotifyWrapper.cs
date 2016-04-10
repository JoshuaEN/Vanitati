using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnUserRemovedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.UserRemovedEventArgs Args { get; }

        public OnUserRemovedNotifyWrapper(Game.Event.UserRemovedEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnUserRemoved(this, Args);
        }
    }
}
