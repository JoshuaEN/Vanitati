using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GenericContext : Context
    {
        public override bool CanBeSource
        {
            get { return false; }
        }
        public override ActionType.Category ActionCategory
        {
            get { throw new NotSupportedException(); }
        }

        public override ActionType.TargetCategory ActionTargetCategory
        {
            get { return ActionType.TargetCategory.Generic; }
        }

        //[Newtonsoft.Json.JsonConverter(typeof(Serializers.JsonConverters.GenericContextConverter))]
        public object Value { get; }
        public string ValueType { get { return Value.GetType().FullName; } }

        public GenericContext(object value)
        {
            Contract.Requires<ArgumentNullException>(null != value);
            Contract.Requires<ArgumentException>(IsValidGenericValueType(value.GetType()));
            Value = value;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Value);
        }

        [Pure]
        public static bool IsValidGenericValueType(Type valueType)
        {
            foreach (var type in ActionType.GENERIC_ACTION_TYPE_VALUES.Values)
            {
                if (type.IsAssignableFrom(valueType))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
