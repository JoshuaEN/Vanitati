using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class AssignUserToCommanderCallWrapper : CallMessageWrapper, AuthInterfaces.IUserAuth
    {
        public int UserIDForAuth { get { return ClaimedSenderUserID; } }
        public override bool RequiresHost { get { return ClaimsHasHost; } }
        public int? UserID { get; }
        public int CommanderID { get; }
        public bool ClaimsHasHost { get; }
        public int ClaimedSenderUserID { get; }

        public AssignUserToCommanderCallWrapper(int? userID, int commanderID, int claimedSenderUserID, bool claimsHasHost)
        {
            UserID = userID;
            CommanderID = commanderID;
            ClaimedSenderUserID = claimedSenderUserID;
            ClaimsHasHost = claimsHasHost;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.AssignUserToCommander(UserID, CommanderID, ClaimedSenderUserID, ClaimsHasHost);
        }
    }
}
