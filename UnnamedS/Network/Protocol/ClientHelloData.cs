using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Protocol
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ClientHelloData
    {
        public string Name { get; }
        public int InitialSyncID { get; }

        public ClientHelloData(string name, int initialSyncID)
        {
            Contract.Requires<ArgumentNullException>(null != name);

            Name = name;
            InitialSyncID = initialSyncID;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Name);
        }
    }
}
