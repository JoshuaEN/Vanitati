using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ClientInfoPacketProtocolWrapper : ServerToClientProtocolMessageWrapper
    {
        public Game.User User { get; }

        public ClientInfoPacketProtocolWrapper(Game.User user)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            User = user;
        }

        public override void Run(IClientProtocolLogic logic)
        {
            logic.ClientInfoPacketRecieved(User);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != User);
        }
    }
}
