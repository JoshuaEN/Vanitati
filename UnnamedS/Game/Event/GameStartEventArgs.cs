using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class GameStartEventArgs : EventArgs
    {
        public BattleGameState.Fields ChangeInfo { get; }

        public GameStartEventArgs(BattleGameState.Fields changeInfo)
        {
            ChangeInfo = changeInfo;
        }
    }
}
