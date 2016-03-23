using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace UnnamedStrategyGame.Game
{
    [ContractClass(typeof(ContractClassForIAttributeDefinition))]
    public interface IAttributeDefinition
    {
        string Key { get; }
        Type Type { get; }
        bool HasDefaultValue { get; }

        [Pure]
        bool CheckAttribute(IAttribute value);

        [Pure]
        bool CheckAttributeDefinition(IAttributeDefinition value);

        [Pure]
        bool ValidateAttributeValue(IAttribute value);

        [Pure]
        IAttribute GetDefaultAttribute(bool readOnly = false);

        [Pure]
        IAttribute GetAttribute(object value, bool readOnly = false);
    }

    [ContractClassFor(typeof(IAttributeDefinition))]
    internal abstract class ContractClassForIAttributeDefinition : IAttributeDefinition
    {
        public abstract bool HasDefaultValue { get; }
        public abstract string Key { get; }
        public abstract Type Type { get; }

        [Pure]
        public bool CheckAttribute(IAttribute value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return false;
        }

        [Pure]
        public bool CheckAttributeDefinition(IAttributeDefinition value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            return false;
        }

        [Pure]
        public IAttribute GetAttribute(object value, bool readOnly = false)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            Contract.Ensures(Contract.Result<IAttribute>() != null);

            return null;
        }

        [Pure]
        public IAttribute GetDefaultAttribute(bool readOnly = false)
        {
            Contract.Requires<InvalidOperationException>(HasDefaultValue, "Attribute Definition does not contain a default value.");
            Contract.Ensures(Contract.Result<IAttribute>() != null);

            return null;
        }

        [Pure]
        public bool ValidateAttributeValue(IAttribute value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            Contract.Requires<Exceptions.IncompatableAttributeException>(CheckAttribute(value));
            return false;
        }
    }
}
