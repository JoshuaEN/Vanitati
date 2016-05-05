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

            return GetInstanceFromTypeString(objectType, key);
        }

        public static object GetInstanceFromTypeString(Type objectType, string key)
        {

            if (typeof(UnitType).IsAssignableFrom(objectType))
                return UnitType.TYPES[key];
            else if (typeof(TerrainType).IsAssignableFrom(objectType))
                return TerrainType.TYPES[key];
            else if (typeof(MovementType).IsAssignableFrom(objectType))
                return MovementType.TYPES[key];
            else if (typeof(ActionType).IsAssignableFrom(objectType))
                return ActionType.TYPES[key];
            else if (typeof(SupplyType).IsAssignableFrom(objectType))
                return SupplyType.TYPES[key];
            else if (typeof(CommanderType).IsAssignableFrom(objectType))
                return CommanderType.TYPES[key];
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
