using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Saving.Wrappers;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class SavingWrapperConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(BaseWrapper) == objectType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var typeStr = (string)json["TypeKey"];
            BaseWrapper wrapper;
            if (BaseWrapper.TYPES.TryGetValue(typeStr, out wrapper) == false)
            {
                throw new ArgumentException("Invalid Save Wrapper Type of " + typeStr);
            }
            return json.ToObject(wrapper.GetType(), serializer);
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
