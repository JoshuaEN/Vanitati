using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    class InvalidDefinitionException : GameException
    {
        public InvalidDefinitionException() : base("Invalid Definition") { }
        public InvalidDefinitionException(string message) : base(message) { }
        public InvalidDefinitionException(string message, Exception innerException) : base(message, innerException) { }
        public InvalidDefinitionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
