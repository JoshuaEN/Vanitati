using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Attribute.Validator
{
    public class UnitTypeString : AttributeValidator
    {
        public override bool SupportsAttributeDefinition(IAttributeDefinition value)
        {
            return value.Key == Globals.ATTRIBUTE_NAME_UNIT_TYPE && value.Type == typeof(string);
        }

        public override bool Validate(IAttribute value)
        {
            return UnitType.TYPES.ContainsKey((string)value.GetValue());
        }
    }
}
