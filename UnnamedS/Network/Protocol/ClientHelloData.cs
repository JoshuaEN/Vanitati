using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Protocol
{
    public class ClientHelloData
    {
        public string Name { get; }
        public int InitialSyncID { get; }

        public ClientHelloData(string name, int initialSyncID)
        {
            Name = name;
            InitialSyncID = initialSyncID;
        }
    }
}
