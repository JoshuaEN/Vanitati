using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class NotYourTurnException : GameException
    {
        public NotYourTurnException() : base() { }
        public NotYourTurnException(string message) : base(message) { }
        public NotYourTurnException(string message, Exception innerException) : base(message, innerException) { }
        public NotYourTurnException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
