using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class ConnectionClosedException : NetworkExcetpion
    {
        public ConnectionClosedException() : base() { }
        public ConnectionClosedException(string message) : base(message) { }
        public ConnectionClosedException(string message, Exception innerException) : base(message, innerException) { }
        public ConnectionClosedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
