using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Attribute;

namespace UnnamedStrategyGame.Game
{
    public class AttributeDefinition<T> : IAttributeDefinition
    {
        public string Key { get; }
        public Type Type { get; }
        public T DefaultValue { get; }
        public bool HasDefaultValue { get; }
        public IReadOnlyList<AttributeValidator> Validators { get; }

        public AttributeDefinition(string key, T defaultValue, List<AttributeValidator> validators = null) : this(key, validators)
        {
            DefaultValue = defaultValue;
            HasDefaultValue = true;
        }

        public AttributeDefinition(string key, List<AttributeValidator> validators = null)
        {
            Key = key;
            Type = typeof(T);
            HasDefaultValue = false;

            if (validators != null)
            {
                Validators = validators.AsReadOnly();

                foreach (var validator in Validators)
                {
                    if (validator.SupportsAttributeDefinition(this) == false)
                    {
                        throw new IncompatableAttributeValidatorException(String.Format("Validator {0} does not support Attribute Definition {1}", validator, this));
                    }
                }
            }
        }

        public bool CheckAttribute(IAttribute value)
        {
            return CheckAttributeDefinition(value.Definition);
        }

        public bool CheckAttributeDefinition(IAttributeDefinition value)
        {
            return Key == value.Key && Type == value.Type;
        }

        public IAttribute GetDefaultAttribute(bool readOnly = false)
        {
            return new Attribute<T>(this, DefaultValue, readOnly);
        }

        public bool ValidateAttributeValue(IAttribute value)
        {
            if (Validators == null)
                return true;

            foreach(var validator in Validators)
            {
                if (validator.Validate(value) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public IAttribute GetAttribute(object value, bool readOnly = false)
        {
            if(value is T == false)
            {
                throw new Exceptions.BadAttributeTypeException(String.Format("Value of {0} for Attribute {1} is of the wrong Type ({2} instead of {3})", value, Key, value.GetType(), typeof(T)));
            }

            return new Attribute<T>(this, (T)value, readOnly);
        }
    }

    public class IncompatableAttributeValidatorException : Exceptions.GameAttributeException
    {
        public IncompatableAttributeValidatorException() : base("Incompatable Attribute Validator") { }
        public IncompatableAttributeValidatorException(string message) : base(message) { }
        public IncompatableAttributeValidatorException(string message, Exception innerException) : base(message, innerException) { }
        public IncompatableAttributeValidatorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
