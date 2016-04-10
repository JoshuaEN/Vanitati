using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnTurnEndedNotifyWrapper : NotifyMessageWrapper
    {
        public TurnEndedEventArgs Args { get; }

        public OnTurnEndedNotifyWrapper(TurnEndedEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnTurnEnded(this, Args);
        }
    }
}
