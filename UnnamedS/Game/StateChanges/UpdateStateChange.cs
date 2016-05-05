using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class UpdateStateChange : StateChange
    {
        [Newtonsoft.Json.JsonConverter(typeof(Serializers.JsonConverters.DynamicProperitesConverter))]
        public IDictionary<string, object> UpdatedProperties { get; }

        public UpdateStateChange(IDictionary<string, object> updatedProperties)
        {
            Contract.Requires<ArgumentNullException>(null != updatedProperties);
            UpdatedProperties = updatedProperties;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != UpdatedProperties);
        }
    }
}
