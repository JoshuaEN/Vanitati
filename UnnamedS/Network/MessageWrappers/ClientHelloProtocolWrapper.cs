using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ClientHelloProtocolWrapper : ClientToServerProtocolMessageWrapper
    {
        public Protocol.ClientHelloData ClientHelloData { get; }

        public ClientHelloProtocolWrapper(Protocol.ClientHelloData clientHelloData)
        {
            Contract.Requires<ArgumentNullException>(clientHelloData != null);
            ClientHelloData = clientHelloData;
        }

        public override void Run(IServerProtocolLogic logic)
        {
            logic.ClientHelloReceived(ClientHelloData);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != ClientHelloData);
        }
    }
}
