using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class DoActionsCallWrapper : CallMessageWrapper
    {
        public List<ActionInfo> Actions { get; }

        public DoActionsCallWrapper(List<ActionInfo> actions)
        {
            Actions = actions;
        }

        public override void Call(uint playerId, LocalGameLogic logic)
        {
            logic.DoActions(playerId, Actions);
        }
    }
}
