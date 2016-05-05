using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.ActionTypes;

namespace UnnamedStrategyGame.Serializers.JsonConverters
{
    public class ActionTriggerEnumConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var enumTypeString = json.Value<string>("TriggerType");
            var enumValue = json.Value<int>("Trigger");

            if(typeof(UnitAction.ActionTriggers).FullName == enumTypeString)
            {
                return new Game.Action.ActionContext.GenericActionTrigger(ParseEnum<UnitAction.ActionTriggers>(enumValue));
            }
            else if (typeof(TerrainAction.ActionTriggers).FullName == enumTypeString)
            {
                return new Game.Action.ActionContext.GenericActionTrigger(ParseEnum<TerrainAction.ActionTriggers>(enumValue));
            }
            else if (typeof(CommanderAction.ActionTriggers).FullName == enumTypeString)
            {
                return new Game.Action.ActionContext.GenericActionTrigger(ParseEnum<CommanderAction.ActionTriggers>(enumValue));
            }
            else if (typeof(GameAction.ActionTriggers).FullName == enumTypeString)
            {
                return new Game.Action.ActionContext.GenericActionTrigger(ParseEnum<GameAction.ActionTriggers>(enumValue));
            }
            else
            {
                throw new ArgumentException($"Unknown type string {enumTypeString}");
            }

        }

        private T ParseEnum<T>(int val)
        {
            var enumType = typeof(T);
            if(enumType.IsEnumDefined(val))
            {
                return (T)Enum.Parse(enumType, enumType.GetEnumName(val));
            }
            else
            {
                throw new ArgumentException($"Invalid enum value of {val} for enum type of {typeof(T)}");
            }
        }

        public override bool CanWrite { get; } = false;

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
