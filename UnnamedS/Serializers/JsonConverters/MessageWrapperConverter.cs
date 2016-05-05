using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Network.MessageWrappers;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class MessageWrapperConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MessageWrapper) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var typeStr = (string)json["Type"];
            Type type;
            if(MessageWrapper.WRAPPER_LISTING.TryGetValue(typeStr, out type) == false)
            {
                throw new ArgumentException("Invalid Message Wrapper Type of " + typeStr);
            }
            return json.ToObject(type, serializer);
            //            return serializer.Deserialize(reader, type);
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
            throw new NotSupportedException();
        }
    }
}
