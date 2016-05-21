using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CommanderTypeCallWrapper : CallMessageWrapper, AuthInterfaces.ICommanderAuth
    {
        public int CommanderID { get; }

        public CommanderTypeCallWrapper(int commanderID)
        {
            CommanderID = commanderID;
        }

        public virtual bool CommanderAuthCheck(LocalGameLogic logic, User user)
        {
            Contract.Requires<ArgumentNullException>(null != logic);
            Contract.Requires<ArgumentNullException>(null != user);

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
