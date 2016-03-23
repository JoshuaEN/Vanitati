using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
