using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Exceptions
{
    public class IncompleteMessageException : ProtocolException
    {
        public IncompleteMessageException() : base() { }
        public IncompleteMessageException(string message) : base(message) { }
        public IncompleteMessageException(string message, Exception innerException) : base(message, innerException) { }
        public IncompleteMessageException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
