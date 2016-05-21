using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DoActionsCallWrapper : CommanderTypeCallWrapper, AuthInterfaces.IUserAuth
    {
        public override bool RequiresHost { get; } = false;
        public Game.Event.ActionIdentifyingInfo ActionIdentifyingInfo { get; }
        public List<ActionInfo> Actions { get; }

        public int UserIDForAuth { get { return ActionIdentifyingInfo.UserID; } }

        public DoActionsCallWrapper(Game.Event.ActionIdentifyingInfo actionIdentifyingInfo, List<ActionInfo> actions) : base(-1)
        {
            Contract.Requires<ArgumentNullException>(null != actions);
            Contract.Requires<ArgumentNullException>(null != actionIdentifyingInfo);
            ActionIdentifyingInfo = actionIdentifyingInfo;
            Actions = actions;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.DoActions(ActionIdentifyingInfo, Actions);
        }

        public override bool CommanderAuthCheck(LocalGameLogic logic, User user)
        {
            int? commanderID = null;

            foreach (var action in Actions)
            {
                if (action.Context.TriggeredByCommanderID == null)
                    return false;

                int tmpCommanderID = (int)action.Context.TriggeredByCommanderID;
                if (commanderID == null)
                {
                    commanderID = tmpCommanderID;
                }
                else if (commanderID != tmpCommanderID)
                {
                    // Sending commands from more than one commander is not permitted.
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

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Actions);
            Contract.Invariant(null != ActionIdentifyingInfo);
        }
    }
}
