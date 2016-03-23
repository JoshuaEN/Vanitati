using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    /// <summary>
    /// Generic Attribute Exception
    /// </summary>
    public abstract class GameAttributeException : GameException
    {
        public GameAttributeException() : base() { }
        public GameAttributeException(string message) : base(message) { }
        public GameAttributeException(string message, Exception innerException) : base(message, innerException) { }
        public GameAttributeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when two Incompatable Attributes are used together.
    /// </summary>
    public class IncompatableAttributeException : GameAttributeException
    {
        public IncompatableAttributeException() : base("Incompatable Attribute") { }
        public IncompatableAttributeException(string message) : base(message) { }
        public IncompatableAttributeException(string message, Exception innerException) : base(message, innerException) { }
        public IncompatableAttributeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an attribute without a default value is missing.
    /// </summary>
    public class MissingAttributeException : GameAttributeException
    {
        public MissingAttributeException() : base("Missing Attribute") { }
        public MissingAttributeException(string message) : base(message) { }
        public MissingAttributeException(string message, Exception innerException) : base(message, innerException) { }
        public MissingAttributeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when attempting to access an unknown attribute.
    /// </summary>
    public class UnknownAttributeException : GameAttributeException
    {
        public UnknownAttributeException() : base("Unknown Attribute") { }
        public UnknownAttributeException(string message) : base(message) { }
        public UnknownAttributeException(string message, Exception innerException) : base(message, innerException) { }
        public UnknownAttributeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an attribute does not match it's definition.
    /// </summary>
    public class AttributeDefinitionMismatchException : GameAttributeException
    {
        public AttributeDefinitionMismatchException() : base("Attribute Definition Mismatch") { }
        public AttributeDefinitionMismatchException(string message) : base(message) { }
        public AttributeDefinitionMismatchException(string message, Exception innerException) : base(message, innerException) { }
        public AttributeDefinitionMismatchException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when attempting to create a new Attribute from a Definition with an invalid input.
    /// </summary>
    public class BadAttributeTypeException : GameAttributeException
    {
        public BadAttributeTypeException() : base("Bad Attribute Type") { }
        public BadAttributeTypeException(string message) : base(message) { }
        public BadAttributeTypeException(string message, Exception innerException) : base(message, innerException) { }
        public BadAttributeTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when attempting to reassign a read-only attribute
    /// </summary>
    public class AttributeReadOnlyException : GameAttributeException
    {
        public AttributeReadOnlyException() : base("Attribute Read-Only") { }
        public AttributeReadOnlyException(string message) : base(message) { }
        public AttributeReadOnlyException(string message, Exception innerException) : base(message, innerException) { }
        public AttributeReadOnlyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
