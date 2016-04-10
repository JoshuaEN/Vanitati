using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    class StartGameCallWrapper : CallMessageWrapper
    {
        public BattleGameState.Fields ChangeInfo { get; }

        public StartGameCallWrapper(BattleGameState.Fields changeInfo) : base(true)
        {
            ChangeInfo = changeInfo;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.StartGame(ChangeInfo);
        }
    }
}
