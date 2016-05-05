using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ConnectionGracefullyClosedException : ConnectionClosedException
    {
        public ConnectionGracefullyClosedException() : base() { }
        public ConnectionGracefullyClosedException(string message) : base(message) { }
        public ConnectionGracefullyClosedException(string message, Exception innerException) : base(message, innerException) { }
        public ConnectionGracefullyClosedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
