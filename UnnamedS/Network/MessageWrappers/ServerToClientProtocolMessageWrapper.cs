using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [ContractClass(typeof(ContractClassForServerToClientProtocolMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class ServerToClientProtocolMessageWrapper : ProtocolMessageWrapper
    {
        public abstract void Run(IClientProtocolLogic logic);
    }

    [ContractClassFor(typeof(ServerToClientProtocolMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForServerToClientProtocolMessageWrapper : ServerToClientProtocolMessageWrapper
    {
        public override void Run(IClientProtocolLogic logic)
        {
            Contract.Requires<ArgumentNullException>(null != logic);
        }
    }
}
