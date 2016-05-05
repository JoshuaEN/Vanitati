using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    /// <summary>
    /// Generic Validator Exception
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class ValidatorException : Exceptions.GameException
    {
        public ValidatorException() : base() { }
        public ValidatorException(string message) : base(message) { }
        public ValidatorException(string message, Exception innerException) : base(message, innerException) { }
        public ValidatorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
