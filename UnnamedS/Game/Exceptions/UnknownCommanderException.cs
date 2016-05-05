using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UnknownCommanderException : GameException
    {
        public UnknownCommanderException() : base() { }
        public UnknownCommanderException(string message) : base(message) { }
        public UnknownCommanderException(string message, Exception innerException) : base(message, innerException) { }
        public UnknownCommanderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
