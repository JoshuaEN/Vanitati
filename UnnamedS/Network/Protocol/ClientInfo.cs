﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Protocol
{
    public class ClientInfo
    {
        public uint PlayerId { get; }
        public bool IsHost { get; }

        public ClientInfo(uint playerId, bool isHost)
        {
            PlayerId = playerId;
            IsHost = isHost;
        }
    }
}
