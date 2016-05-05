using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TerrainStateChange : UpdateStateChange
    {
        public Location Location { get; }
        public TerrainStateChange(Location location, IDictionary<string, object> updatedProperties) : base(updatedProperties)
        {
            Contract.Requires<ArgumentNullException>(null != location);
            Location = location;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Location);
        }
    }
}
