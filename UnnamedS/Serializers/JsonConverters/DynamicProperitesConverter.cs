using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class DynamicProperitesConverter : JsonConverter
    {
        private static readonly IReadOnlyDictionary<string, Type> propertyData;

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        static DynamicProperitesConverter()
        {
            Dictionary<string, Type> tmpPropertyData = new Dictionary<string, Type>();

            AddPropertyData(Game.Properties.PropertyContainer.UNIT.PropertyData, ref tmpPropertyData);
            AddPropertyData(Game.Properties.PropertyContainer.TERRAIN.PropertyData, ref tmpPropertyData);
            AddPropertyData(Game.Properties.PropertyContainer.COMMANDER.PropertyData, ref tmpPropertyData);
            AddPropertyData(Game.Properties.PropertyContainer.BATTLE_GAME_STATE.PropertyData, ref tmpPropertyData);

            propertyData = tmpPropertyData;
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private static void AddPropertyData(IReadOnlyDictionary<string, Game.Properties.PropData> data, ref Dictionary<string, Type> refPropertyData)
        {
            foreach(var v in data)
            {
                Type t;
                if(refPropertyData.TryGetValue(v.Key, out t))
                {
                    if (t != v.Value.Type)
                        throw new ArgumentException(string.Format("Incompatible types {0}, {1} from same key of {2}", t, v.Value.Type, v.Key));
                }
                else
                {
                    refPropertyData.Add(v.Key, v.Value.Type);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var dic = new Dictionary<string, object>();

            foreach(var child in json.Children<JProperty>())
            {
                Type propType;

                if (propertyData.TryGetValue(child.Name, out propType) == false)
                    throw new ArgumentException(string.Format("Invalid property key of {0} for type {1}", child.Name, objectType.Name));

                dic.Add(child.Name, child.Value.ToObject(propType, serializer));
            }

            return dic;
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
