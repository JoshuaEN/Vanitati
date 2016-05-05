using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UnknownPlayerException : GameException
    {
        public UnknownPlayerException() : base() { }
        public UnknownPlayerException(string message) : base(message) { }
        public UnknownPlayerException(string message, Exception innerException) : base(message, innerException) { }
        public UnknownPlayerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
