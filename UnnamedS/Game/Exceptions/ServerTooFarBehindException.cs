using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    public class ServerTooFarBehindException : GameException
    {
        public ServerTooFarBehindException() : base() { }
        public ServerTooFarBehindException(string message) : base(message) { }
        public ServerTooFarBehindException(string message, Exception innerException) : base(message, innerException) { }
        public ServerTooFarBehindException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
