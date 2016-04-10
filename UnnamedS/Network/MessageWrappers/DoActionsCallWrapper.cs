using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class DoActionsCallWrapper : CommanderTypeCallWrapper
    {
        public List<ActionInfo> Actions { get; }

        public DoActionsCallWrapper(List<ActionInfo> actions) : base(-1)
        {
            Actions = actions;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.DoActions(Actions);
        }

        public override bool AuthCheck(LocalGameLogic logic, User user)
        {
            int? commanderID = null;

            foreach(var action in Actions)
            {
                if(commanderID == null)
                {
                    commanderID = action.CommanderID;
                }
                else if(commanderID != action.CommanderID)
                {
                    return false;
                }
            }

            if(commanderID == null)
            {
                return false;
            }

            int? assignedUserID;
            if (logic.CommanderAssignments.TryGetValue((int)commanderID, out assignedUserID))
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
