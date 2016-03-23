using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnThisPlayerAddedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.OnThisPlayerAddedArgs Args { get; }

        public OnThisPlayerAddedNotifyWrapper(Game.Event.OnThisPlayerAddedArgs args)
        {
            Args = args;
        }

        public override void Notify(IPlayerLogic logic)
        {
            logic.OnThisPlayerAdded(this, Args);
        }
    }
}
