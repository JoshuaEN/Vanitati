using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class ClientHelloProtocolWrapper : ClientToServerProtocolMessageWrapper
    {
        public Protocol.ClientHelloData ClientHelloData { get; }

        public ClientHelloProtocolWrapper(Protocol.ClientHelloData clientHelloData)
        {
            ClientHelloData = clientHelloData;
        }

        public override void Run(IServerProtocolLogic logic)
        {
            logic.ClientHelloReceived(ClientHelloData);
        }
    }
}
