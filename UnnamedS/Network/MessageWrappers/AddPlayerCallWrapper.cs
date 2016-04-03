using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class AddPlayerCallWrapper : CallMessageWrapper
    {
        public override void Call(int playerId, LocalGameLogic logic)
        {
            logic.AddPlayer(null);
        }
    }
}
