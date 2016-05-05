using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class IncompleteHeaderException : ProtocolException
    {
        public IncompleteHeaderException() : base() { }
        public IncompleteHeaderException(string message) : base(message) { }
        public IncompleteHeaderException(string message, Exception innerException) : base(message, innerException) { }
        public IncompleteHeaderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
