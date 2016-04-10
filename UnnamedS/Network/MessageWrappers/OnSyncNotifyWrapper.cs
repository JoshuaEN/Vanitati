using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnSyncNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.SyncEventArgs Args { get; }

        public OnSyncNotifyWrapper(Game.Event.SyncEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnSync(this, Args);
        }
    }
}
