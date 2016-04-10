using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnUserAddedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.UserAddedEventArgs Args { get; }

        public OnUserAddedNotifyWrapper(Game.Event.UserAddedEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnUserAdded(this, Args);
        }
    }
}
