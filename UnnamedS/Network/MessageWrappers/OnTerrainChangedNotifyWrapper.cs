using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnTerrainChangedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.TerrainChangedEventArgs Args { get; }

        public OnTerrainChangedNotifyWrapper(Game.Event.TerrainChangedEventArgs args)
        {
            Args = args;
        }

        public override void Notify(IPlayerLogic logic)
        {
            logic.OnTerrainChanged(this, Args);
        }
    }
}
