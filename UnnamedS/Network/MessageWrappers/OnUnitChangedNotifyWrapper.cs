using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnUnitChangedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.UnitChangedEventArgs Args { get; }

        public OnUnitChangedNotifyWrapper(Game.Event.UnitChangedEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnUnitChanged(this, Args);
        }
    }
}
