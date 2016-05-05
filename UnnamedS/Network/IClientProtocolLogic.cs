using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network
{
    [ContractClass(typeof(ContractClassForIClientProtocolLogic))]
    public interface IClientProtocolLogic : IProtocolLogic
    {
        void ClientInfoPacketRecieved(User user);
    }

    [ContractClassFor(typeof(IClientProtocolLogic))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForIClientProtocolLogic : IClientProtocolLogic
    {
        public void ClientInfoPacketRecieved(User user)
        {
            Contract.Requires<ArgumentNullException>(null != user);
        }
    }
}
