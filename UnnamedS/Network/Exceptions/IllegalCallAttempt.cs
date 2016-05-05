using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class IllegalCallAttempt : ProtocolException
    {
        public IllegalCallAttempt() : base() { }
        public IllegalCallAttempt(string message) : base(message) { }
        public IllegalCallAttempt(string message, Exception innerException) : base(message, innerException) { }
        public IllegalCallAttempt(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
