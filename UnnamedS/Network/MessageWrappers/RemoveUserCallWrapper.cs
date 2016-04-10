using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class RemoveUserCallWrapper : CallMessageWrapper
    {
        public int UserID { get; }

        public RemoveUserCallWrapper(int userID)
        {
            UserID = userID;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.RemoveUser(UserID);
        }
    }
}
