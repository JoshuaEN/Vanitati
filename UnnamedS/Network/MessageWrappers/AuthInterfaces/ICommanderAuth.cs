using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers.AuthInterfaces
{
    interface ICommanderAuth
    {
        bool CommanderAuthCheck(LocalGameLogic logic, User user);
    }
}
