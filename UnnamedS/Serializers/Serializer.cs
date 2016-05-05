using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Serializers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class Serializer
    {
        public static BaseSerializer Instance { get; } = new JsonSerializer();

        public static T Deserialize<T>(string str)
        {
            Contract.Requires<ArgumentNullException>(null != str);
            Contract.Ensures(Contract.Result<T>() != null);

            return Instance.Deserialize<T>(str);
        }

        public static object Deserialize(string str, Type type)
        {
            Contract.Requires<ArgumentNullException>(null != str);
            Contract.Requires<ArgumentNullException>(null != type);
            Contract.Ensures(Contract.Result<object>() != null);

            return Instance.Deserialize(str, type);
        }

        public static string Serialize(object obj)
        {
            Contract.Requires<ArgumentNullException>(null != obj);
            Contract.Ensures(Contract.Result<string>() != null);

            return Instance.Serialize(obj);
        }
    }
}
