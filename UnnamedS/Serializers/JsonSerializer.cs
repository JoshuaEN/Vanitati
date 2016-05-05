using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Serializers
{
    public class JsonSerializer : BaseSerializer
    {
        public static readonly Newtonsoft.Json.JsonSerializer SERIALIZER;

        static JsonSerializer()
        {
            SERIALIZER = new Newtonsoft.Json.JsonSerializer();

            SERIALIZER.NullValueHandling = NullValueHandling.Ignore;
            SERIALIZER.Converters.Add(new JsonConverters.XTypeJsonConverter());
            SERIALIZER.Converters.Add(new JsonConverters.XTypeDictionaryConverter());
            SERIALIZER.Converters.Add(new JsonConverters.MessageWrapperConverter());
            SERIALIZER.Converters.Add(new JsonConverters.ContextConverter());
            SERIALIZER.Converters.Add(new JsonConverters.GenericContextConverter());

            //SERIALIZER.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            //SERIALIZER.PreserveReferencesHandling = PreserveReferencesHandling.All;

#if DEBUG
            SERIALIZER.Formatting = Formatting.Indented;
#else
            SERIALIZER.Formatting = Formatting.None;
#endif
        }

        public override T Deserialize<T>(string str)
        {
            using (StringReader sr = new StringReader(str))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return SERIALIZER.Deserialize<T>(reader);
            }
        }

        public override object Deserialize(string str, Type type)
        {
            using (StringReader sr = new StringReader(str))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return SERIALIZER.Deserialize(reader, type);
            }
        }

        public override string Serialize(object obj)
        {
            using (StringWriter sw = new StringWriter())
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                SERIALIZER.Serialize(writer, obj);
                return sw.ToString();
            }
        }
    }
}
