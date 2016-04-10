using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    public interface IClientProtocolLogic : IProtocolLogic
    {
        void ClientInfoPacketRecieved(Game.User user);
    }
}
