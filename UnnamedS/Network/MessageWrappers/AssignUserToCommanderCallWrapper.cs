using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class AssignUserToCommanderCallWrapper : CallMessageWrapper
    {
        public int? UserID { get; }
        public int CommanderID { get; }

        public AssignUserToCommanderCallWrapper(int? userID, int commanderID)
        {
            UserID = userID;
            CommanderID = commanderID;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.AssignUserToCommander(UserID, CommanderID);
        }
    }
}
