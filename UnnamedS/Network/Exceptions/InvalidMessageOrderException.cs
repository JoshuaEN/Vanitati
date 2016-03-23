using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Exceptions
{
    public class InvalidMessageOrderException : ProtocolException
    {
        public InvalidMessageOrderException() : base() { }
        public InvalidMessageOrderException(string message) : base(message) { }
        public InvalidMessageOrderException(string message, Exception innerException) : base(message, innerException) { }
        public InvalidMessageOrderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
