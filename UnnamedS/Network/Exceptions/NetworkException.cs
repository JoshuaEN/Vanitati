using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Network.Exceptions
{
    public class NetworkExcetpion : Exception
    {
        public NetworkExcetpion() : base() { }
        public NetworkExcetpion(string message) : base(message) { }
        public NetworkExcetpion(string message, Exception innerException) : base(message, innerException) { }
        public NetworkExcetpion(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
