using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public abstract class CommanderTypeCallWrapper : CallMessageWrapper
    {
        public int CommanderID { get; }

        public CommanderTypeCallWrapper(int commanderID)
        {
            CommanderID = commanderID;
        }

        public virtual bool AuthCheck(LocalGameLogic logic, User user)
        {
            int? assignedUserID;
            if(logic.CommanderAssignments.TryGetValue(CommanderID, out assignedUserID))
            {
                return assignedUserID == user.UserID;
            }
            else
            {
                return false;
            }
        }
    }
}
