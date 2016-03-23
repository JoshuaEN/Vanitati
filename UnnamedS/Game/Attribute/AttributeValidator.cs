using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Attribute
{
    public abstract class AttributeValidator
    {
        public abstract bool SupportsAttributeDefinition(IAttributeDefinition value);
        public abstract bool Validate(IAttribute value);
    }
}
