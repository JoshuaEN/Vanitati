using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Properties
{
    public sealed class PropertyContainer
    {
        public static readonly Helper<BattleGameState> BATTLE_GAME_STATE = new Helper<BattleGameState>();
        public static readonly Helper<Unit> UNIT = new Helper<Unit>();
        public static readonly Helper<Terrain> TERRAIN = new Helper<Terrain>();
        public static readonly Helper<Commander> COMMANDER = new Helper<Commander>();

        public sealed class Helper<T>
        {
            private Dictionary<string, PropertyInfo> ReadableProperties { get; } = new Dictionary<string, PropertyInfo>();
            private Dictionary<string, PropertyInfo> WritableProperties { get; } = new Dictionary<string, PropertyInfo>();
            private Dictionary<string, PropData> Properties { get; } = new Dictionary<string, PropData>();

            public IReadOnlyDictionary<string, PropData> PropertyData
            {
                get { return Properties; }
            }

            public Helper()
            {
                var info = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => Attribute.IsDefined(p, typeof(Newtonsoft.Json.JsonIgnoreAttribute)) == false);

                foreach (var i in info)
                {
                    var readable = false;
                    var writeable = false;

                    var get = i.GetGetMethod(true);
                    if (null != get)
                    {
                        ReadableProperties.Add(i.Name, i);
                        readable = true;
                    }
                    var set = i.GetSetMethod(true);
                    if (null != set)
                    {
                        WritableProperties.Add(i.Name, i);
                        writeable = true;
                    }

                    if (readable || writeable)
                    {
                        Properties.Add(i.Name, new PropData(i, readable, writeable));
                    }
                }
            }


            public IDictionary<string, object> GetProperties(T instance)
            {
                var dic = new Dictionary<string, object>();
                foreach (var prop in ReadableProperties)
                {
                    dic.Add(prop.Key, prop.Value.GetValue(instance));
                }
                return dic;
            }

            public IDictionary<string, object> GetWriteableProperties(T instance)
            {
                var dic = new Dictionary<string, object>();
                foreach(var prop in Properties.Where(p => p.Value.Readable && p.Value.Writeable))
                {
                    dic.Add(prop.Key, prop.Value.Info.GetValue(instance));
                }
                return dic;
            }

            public void SetProperties(T instance, IDictionary<string, object> values)
            {
                foreach (var item in values)
                {
                    PropertyInfo info;
                    if (WritableProperties.TryGetValue(item.Key, out info) == false)
                    {
                        if (ReadableProperties.ContainsKey(item.Key))
                        {
                            throw new Game.Exceptions.PropertyReadOnlyException(String.Format("Property {0} is not a writable property", item.Key));
                        }
                        else
                        {
                            throw new Game.Exceptions.UnknownPropertyException(String.Format("Property {0} is not known for type {1}", item.Key, typeof(T).Name));
                        }
                    }

                    if (info.PropertyType.IsAssignableFrom(item.Value.GetType()) == false)
                    {
                        throw new Exceptions.IncompatiblePropertyException(string.Format("Attempted to set property {0} to unsupported type of {1} (property is of type {2}", item.Key, item.Value.GetType().Name, info.PropertyType.Name));
                    }

                    info.SetValue(instance, item.Value);
                }
            }
        }
    }
}
