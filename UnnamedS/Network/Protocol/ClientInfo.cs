using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Protocol
{
    public class ClientInfo
    {
        public int PlayerId { get; }
        public bool IsHost { get; }

        public ClientInfo(int playerId, bool isHost)
        {
            PlayerId = playerId;
            IsHost = isHost;
        }
    }
}
