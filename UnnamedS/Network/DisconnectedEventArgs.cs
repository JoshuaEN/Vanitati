using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DisconnectedEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public DisconnectedEventArgs(Exception exception)
        {
            Contract.Requires<ArgumentNullException>(null != exception);

            Exception = exception;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Exception);
        }
    }
}
