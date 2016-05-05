using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Properties
{
    [ContractClass(typeof(ContractClassForIPropertyContainer))]
    public interface IPropertyContainer
    {
        IDictionary<string, object> GetProperties();
        IDictionary<string, object> GetWriteableProperties();
        void SetProperties(IDictionary<string, object> values);
    }

    [ContractClassFor(typeof(IPropertyContainer))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForIPropertyContainer : IPropertyContainer
    {
        public IDictionary<string, object> GetProperties()
        {
            Contract.Ensures(Contract.Result<IDictionary<string, object>>() != null);
            return null;
        }

        public IDictionary<string, object> GetWriteableProperties()
        {
            Contract.Ensures(Contract.Result<IDictionary<string, object>>() != null);
            return null;
        }

        public void SetProperties(IDictionary<string, object> values)
        {
            Contract.Requires<ArgumentNullException>(null != values);
        }
    }
}
