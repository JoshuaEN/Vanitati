using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class XTypeJsonConverter : Newtonsoft.Json.JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseType).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var key = (string)reader.Value;

            if (objectType == typeof(UnitType))
                return UnitType.TYPES[key];
            else if (objectType == typeof(TerrainType))
                return TerrainType.TYPES[key];
            else if (objectType == typeof(MovementType))
                return MovementType.TYPES[key];
            else if (objectType == typeof(ActionType))
                return ActionType.TYPES[key];
            else
                throw new InvalidOperationException(string.Format("Unsupported or unknown type of {0}", objectType.Name));
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if(value is BaseType)
            {
                writer.WriteValue((value as BaseType).Key);
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unsupported or unknown type of {0}", value.GetType().Name));
            }

        }
    }
}
