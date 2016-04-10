using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class XTypeDictionaryConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsGenericType &&
                typeof(Game.BaseType).IsAssignableFrom(objectType.GetGenericArguments()[0]) &&
                (
                    (objectType.GetGenericTypeDefinition() == typeof(IDictionary<,>) || objectType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) ||
                    objectType.GetInterfaces().Any(i => i == typeof(System.Collections.IDictionary) || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
                )
                ;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var instance = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(objectType.GetGenericArguments()));
            var dic = (instance as System.Collections.IDictionary);

            var keyType = objectType.GetGenericArguments()[0];
            var valueType = objectType.GetGenericArguments()[1];

            foreach (var child in json.Children())
            {
                var prop = (child as JProperty);
                dic.Add(XTypeJsonConverter.GetInstanceFromTypeString(keyType, prop.Name), prop.Value.ToObject(valueType));
            }

            return dic;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();
            foreach (System.Collections.DictionaryEntry kp in (value as System.Collections.IDictionary))
            {
                writer.WritePropertyName((kp.Key as Game.BaseType).Key);
                writer.WriteRawValue(Serializer.Serialize(kp.Value));
            }
            writer.WriteEndObject();
        }
    }
}
