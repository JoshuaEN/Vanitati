using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [ContractClass(typeof(ContractClassForClientToServerProtocolMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class ClientToServerProtocolMessageWrapper : ProtocolMessageWrapper
    {
        public abstract void Run(IServerProtocolLogic logic);
    }

    [ContractClassFor(typeof(ClientToServerProtocolMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForClientToServerProtocolMessageWrapper : ClientToServerProtocolMessageWrapper
    {
        public override void Run(IServerProtocolLogic logic)
        {
            Contract.Requires<ArgumentNullException>(null != logic);
        }
    }
}
