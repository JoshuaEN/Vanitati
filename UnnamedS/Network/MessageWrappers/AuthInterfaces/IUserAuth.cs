using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers.AuthInterfaces
{
    interface IUserAuth
    {
        int UserIDForAuth { get; }
    }
}
