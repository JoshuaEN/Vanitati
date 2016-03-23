using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Attribute.Validator
{
    public class NumberBetween<T> : AttributeValidator
    {
        public T Minimum { get; }
        public T Maximum { get; }
        public override bool SupportsAttributeDefinition(IAttributeDefinition value)
        {
            return value.Type.IsAssignableFrom(typeof(T));
        }

        public override bool Validate(IAttribute value)
        {
            T tValue = ((Attribute<T>)value).Value;
            var comparer = Comparer<T>.Default;

            if(Minimum != null && comparer.Compare(Minimum, tValue) > 0)
            {
                throw new NumberBetweenValidatorException(String.Format("{0} is less than minimum of {1}", tValue, Minimum));
            }

            if(Maximum != null && comparer.Compare(tValue, Maximum) > 0)
            {
                throw new NumberBetweenValidatorException(String.Format("{0} is greater than maximum of {1}", tValue, Maximum));
            }

            return true;
        }

        public NumberBetween(T minimum, T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }

    /// <summary>
    /// Generic Validator Exception
    /// </summary>
    public class NumberBetweenValidatorException : Exceptions.ValidatorException
    {
        public NumberBetweenValidatorException() : base() { }
        public NumberBetweenValidatorException(string message) : base(message) { }
        public NumberBetweenValidatorException(string message, Exception innerException) : base(message, innerException) { }
        public NumberBetweenValidatorException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
