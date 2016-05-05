using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Exceptions
{
    /// <summary>
    /// Generic Property Exception
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class GamePropertyException : GameException
    {
        public GamePropertyException() : base() { }
        public GamePropertyException(string message) : base(message) { }
        public GamePropertyException(string message, Exception innerException) : base(message, innerException) { }
        public GamePropertyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when two Incompatible Property's are used together.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class IncompatiblePropertyException : GamePropertyException
    {
        public IncompatiblePropertyException() : base("Incompatible Property") { }
        public IncompatiblePropertyException(string message) : base(message) { }
        public IncompatiblePropertyException(string message, Exception innerException) : base(message, innerException) { }
        public IncompatiblePropertyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an Property without a default value is missing.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MissingPropertyException : GamePropertyException
    {
        public MissingPropertyException() : base("Missing Property") { }
        public MissingPropertyException(string message) : base(message) { }
        public MissingPropertyException(string message, Exception innerException) : base(message, innerException) { }
        public MissingPropertyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when attempting to access an unknown Property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UnknownPropertyException : GamePropertyException
    {
        public UnknownPropertyException() : base("Unknown Property") { }
        public UnknownPropertyException(string message) : base(message) { }
        public UnknownPropertyException(string message, Exception innerException) : base(message, innerException) { }
        public UnknownPropertyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an Property does not match it's definition.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class PropertyDefinitionMismatchException : GamePropertyException
    {
        public PropertyDefinitionMismatchException() : base("Property Definition Mismatch") { }
        public PropertyDefinitionMismatchException(string message) : base(message) { }
        public PropertyDefinitionMismatchException(string message, Exception innerException) : base(message, innerException) { }
        public PropertyDefinitionMismatchException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when attempting to create a new Property from a Definition with an invalid input.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BadPropertyTypeException : GamePropertyException
    {
        public BadPropertyTypeException() : base("Bad Property Type") { }
        public BadPropertyTypeException(string message) : base(message) { }
        public BadPropertyTypeException(string message, Exception innerException) : base(message, innerException) { }
        public BadPropertyTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when attempting to reassign a read-only Property
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class PropertyReadOnlyException : GamePropertyException
    {
        public PropertyReadOnlyException() : base("Property Read-Only") { }
        public PropertyReadOnlyException(string message) : base(message) { }
        public PropertyReadOnlyException(string message, Exception innerException) : base(message, innerException) { }
        public PropertyReadOnlyException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
