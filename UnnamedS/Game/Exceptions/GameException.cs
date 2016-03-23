using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    public abstract class GameException : Exception
    {
        public GameException() : base() { }
        public GameException(string message) : base(message) { }
        public GameException(string message, Exception innerException) : base(message, innerException) { }
        public GameException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
