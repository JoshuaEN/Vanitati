using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class ClientInfoPacketProtocolWrapper : ServerToClientProtocolMessageWrapper
    {
        public Game.User User { get; }

        public ClientInfoPacketProtocolWrapper(Game.User user)
        {
            User = user;
        }

        public override void Run(IClientProtocolLogic logic)
        {
            logic.ClientInfoPacketRecieved(User);
        }
    }
}
