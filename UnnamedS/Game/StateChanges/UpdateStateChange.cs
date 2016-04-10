using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public abstract class UpdateStateChange : StateChange
    {
        [Newtonsoft.Json.JsonConverter(typeof(Serializers.JsonConverters.DynamicProperitesConverter))]
        public IDictionary<string, object> UpdatedProperties { get; }

        public UpdateStateChange(IDictionary<string, object> updatedProperties)
        {
            UpdatedProperties = updatedProperties;
        }
    }
}
