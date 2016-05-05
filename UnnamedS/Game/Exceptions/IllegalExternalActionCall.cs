using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class IllegalExternalActionCall : GameException
    {
        public IllegalExternalActionCall() : base() { }
        public IllegalExternalActionCall(string message) : base(message) { }
        public IllegalExternalActionCall(string message, Exception innerException) : base(message, innerException) { }
        public IllegalExternalActionCall(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
