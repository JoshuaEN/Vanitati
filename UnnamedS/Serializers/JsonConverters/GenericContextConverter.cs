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
            return typeof(TargetContext).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var valueTypes = (JArray)json["ValueTypes"];
            var values = (JArray)json["Values"];

            if(valueTypes.Count != values.Count)
            {
                throw new ArgumentException($"Context Value Mismatch; expected values array length ({values.Count}) to equal value types array length ({valueTypes.Count})");
            }

            object[] parsedValues = new object[valueTypes.Count];

            for (var i = 0; i < valueTypes.Count; i++)
            {
                var typeStr = valueTypes[i].ToObject<string>();
                var value = values[i];

                Type canidateType = Assembly.GetEntryAssembly().GetType(typeStr, false);

                if (canidateType == null)
                {
                    throw new ArgumentException($"Unknown type of {typeStr}");
                }

                foreach (var type in ActionType.GENERIC_ACTION_TYPE_VALUES.Values)
                {
                    if (type.IsAssignableFrom(canidateType))
                    {
                        parsedValues[i] = value.ToObject(canidateType, serializer);
                        break;
                    }
                }

                if(parsedValues[i] == null)
                    throw new ArgumentException("Invalid Generic Context Type Value of " + typeStr);
            }

            return new GenericContext(parsedValues);
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
