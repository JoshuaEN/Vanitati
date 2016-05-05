using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Network.Protocol;

namespace UnnamedStrategyGame.Network
{
    [ContractClass(typeof(ContractClassForIServerProtocolLogic))]
    public interface IServerProtocolLogic : IProtocolLogic
    {
        void ClientHelloReceived(ClientHelloData clientHello);
    }

    [ContractClassFor(typeof(IServerProtocolLogic))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForIServerProtocolLogic : IServerProtocolLogic
    {
        public void ClientHelloReceived(ClientHelloData clientHello)
        {
            Contract.Requires<ArgumentNullException>(null != clientHello);
        }
    }
}
