using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Serializers
{
    public static class Serializer
    {
        public static BaseSerializer Instance { get; } = new JsonSerializer();

        public static T Deserialize<T>(string str)
        {
            return Instance.Deserialize<T>(str);
        }

        public static object Deserialize(string str, Type type)
        {
            return Instance.Deserialize(str, type);
        }

        public static string Serialize(object obj)
        {
            return Instance.Serialize(obj);
        }
    }
}
