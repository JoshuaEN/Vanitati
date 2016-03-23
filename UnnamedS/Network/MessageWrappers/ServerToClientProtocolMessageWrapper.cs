using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public abstract class ServerToClientProtocolMessageWrapper : ProtocolMessageWrapper
    {
        public abstract void Run(IClientProtocolLogic logic);
    }
}
