using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class StateMismatchException : GameException
    {
        public StateMismatchException() : base() { }
        public StateMismatchException(string message) : base(message) { }
        public StateMismatchException(string message, Exception innerException) : base(message, innerException) { }
        public StateMismatchException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
