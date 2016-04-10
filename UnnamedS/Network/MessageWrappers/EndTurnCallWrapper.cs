using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class EndTurnCallWrapper : CommanderTypeCallWrapper
    {
        public EndTurnCallWrapper(int commanderID) : base(commanderID)
        {

        }

        public override void Call(LocalGameLogic logic)
        {
            logic.EndTurn(CommanderID);
        }
    }
}
