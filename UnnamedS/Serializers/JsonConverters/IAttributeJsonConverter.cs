using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class IAttributeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IAttribute).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            IAttributeDefinition def = null;
            object value = null;
            bool readOnly = false;

            JsonToken token;
            do
            {
                token = reader.TokenType;

                if (token == JsonToken.EndObject)
                {
                    break;
                }
                else if (token == JsonToken.PropertyName)
                {
                    var name = (string)reader.Value;

                    if (name == "Value")
                    {
                        reader.Read();
                        value = JsonSerializer.SERIALIZER.Deserialize(reader, def.Type);
                    }
                    else if (name == "Definition")
                    {
                        def = BaseType.AttributeDefinitions[reader.ReadAsString()];
                    }
                    else if(name == "ReadOnly")
                    {
                        readOnly = (bool)reader.ReadAsBoolean();
                    }
                    else
                    {
                        throw new InvalidOperationException("Unsupported Property Token of " + name);
                    }
                    
                }

                reader.Read();
            }
            while (true);

            return def.GetAttribute(value, readOnly);
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
            //serializer.Serialize(writer, value);
            //JsonSerializer.SERIALIZER.Serialize(writer, value);
        }
    }
}
