using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnGameStateChangedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.OnGameStateChangedArgs Args { get; }

        public OnGameStateChangedNotifyWrapper(Game.Event.OnGameStateChangedArgs args)
        {
            Args = args;
        }

        public override void Notify(IPlayerLogic logic)
        {
            logic.OnGameStateChanged(this, Args);
        }
    }
}
