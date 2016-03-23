using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class ClientInfoPacketProtocolWrapper : ServerToClientProtocolMessageWrapper
    {
        public Protocol.ClientInfo ClientInfo { get; }

        public ClientInfoPacketProtocolWrapper(Protocol.ClientInfo clientInfo)
        {
            ClientInfo = clientInfo;
        }

        public override void Run(IClientProtocolLogic logic)
        {
            logic.ClientInfoPacketRecieved(ClientInfo);
        }
    }
}
