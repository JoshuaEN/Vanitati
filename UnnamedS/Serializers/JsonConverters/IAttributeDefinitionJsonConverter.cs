using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    class IAttributeDefinitionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IAttributeDefinition).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return BaseType.AttributeDefinitions[(string)reader.Value];
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is IAttributeDefinition)
            {
                var def = (value as IAttributeDefinition);
                writer.WriteValue(def.Key);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unsupported or unknown type of {0}", value.GetType().Name));
            }
        }
    }
}
