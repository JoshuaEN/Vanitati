using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnGameStartNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.GameStartEventArgs Args { get; }

        public OnGameStartNotifyWrapper(Game.Event.GameStartEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IPlayerLogic logic)
        {
            logic.OnGameStart(this, Args);
        }
    }
}
