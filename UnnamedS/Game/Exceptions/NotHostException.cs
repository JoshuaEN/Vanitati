using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class NotHostException : GameException
    {
        public NotHostException() : base() { }
        public NotHostException(string message) : base(message) { }
        public NotHostException(string message, Exception innerException) : base(message, innerException) { }
        public NotHostException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
