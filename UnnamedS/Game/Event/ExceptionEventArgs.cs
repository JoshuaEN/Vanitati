using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ExceptionEventArgs(Exception exception)
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
