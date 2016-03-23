using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnPlayerChangedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.PlayerChangedEventArgs Args { get; }

        public OnPlayerChangedNotifyWrapper(Game.Event.PlayerChangedEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IPlayerLogic logic)
        {
            logic.OnPlayerChanged(this, Args);
        }
    }
}
