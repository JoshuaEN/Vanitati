using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnnamedStrategyGame.Game.StateChanges;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Action;
using System.Reflection;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class GenericContextConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return typeof(GenericContext).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var typeStr = (string)json["ValueType"];
            Type canidateType = Assembly.GetEntryAssembly().GetType(typeStr, false);

            if(canidateType == null)
            {
                throw new ArgumentException($"Unknown type of {typeStr}");
            }

            foreach(var type in ActionType.GENERIC_ACTION_TYPE_VALUES.Values)
            {
                if(type.IsAssignableFrom(canidateType))
                {
                    return new GenericContext(json["Value"].ToObject(canidateType, serializer));
                }
            }

            throw new ArgumentException("Invalid Generic Context Type Value of " + typeStr);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
