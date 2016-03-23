using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Attribute.Validator
{
    public class TerrainTypeString : AttributeValidator
    {
        public override bool SupportsAttributeDefinition(IAttributeDefinition value)
        {
            return value.Key == Globals.ATTRIBUTE_NAME_TERRAIN_TYPE && value.Type == typeof(string);
        }

        public override bool Validate(IAttribute value)
        {
            return TerrainType.TYPES.ContainsKey((string)value.GetValue());
        }
    }
}
