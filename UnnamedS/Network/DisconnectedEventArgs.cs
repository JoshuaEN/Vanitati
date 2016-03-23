using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    public class DisconnectedEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public DisconnectedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
