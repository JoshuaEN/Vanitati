using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnActionsTakenNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.ActionsTakenEventArgs Args { get; }

        public OnActionsTakenNotifyWrapper(Game.Event.ActionsTakenEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnActionsTaken(this, Args);
        }
    }
}
