using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Serializers
{
    [ContractClass(typeof(ContractClassForBaseSerializer))]
    public abstract class BaseSerializer
    {
        public abstract string Serialize(object obj);
        public abstract T Deserialize<T>(string str);
        public abstract object Deserialize(string str, Type type);
    }

    [ContractClassFor(typeof(BaseSerializer))]
    internal abstract class ContractClassForBaseSerializer : BaseSerializer
    {
        public override string Serialize(object obj)
        {
            Contract.Requires<ArgumentNullException>(null != obj);
            Contract.Ensures(Contract.Result<string>() != null);
            return null;
        }

        public override T Deserialize<T>(string str)
        {
            Contract.Requires<ArgumentNullException>(null != str);
            Contract.Ensures(Contract.Result<T>() != null);

            return default(T);
        }

        public override object Deserialize(string str, Type type)
        {
            Contract.Requires<ArgumentNullException>(null != str);
            Contract.Requires<ArgumentNullException>(null != type);
            Contract.Ensures(Contract.Result<object>() != null);

            return null;
        }
    }
}
