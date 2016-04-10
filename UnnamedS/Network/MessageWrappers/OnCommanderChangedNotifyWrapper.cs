using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnCommanderChangedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.CommanderChangedEventArgs Args { get; }

        public OnCommanderChangedNotifyWrapper(Game.Event.CommanderChangedEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnCommanderChanged(this, Args);
        }
    }
}
