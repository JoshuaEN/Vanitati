using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game
{
    public abstract class StateChange
    {
        public IReadOnlyList<IAttribute> UpdatedAttributes { get; }
        

        public StateChange(List<IAttribute> updatedAttributes)
        {
            UpdatedAttributes = updatedAttributes;
        }

    }
}
