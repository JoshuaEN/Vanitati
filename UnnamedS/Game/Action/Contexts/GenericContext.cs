using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    public class GenericContext : TargetContext
    {
        public override IReadOnlyList<object> Values { get; }
        public override IReadOnlyList<string> ValueTypes { get; }

        public GenericContext(params object[] values)
        {
            Contract.Requires<ArgumentNullException>(null != values);
            Contract.Requires<ArgumentException>(values.ToList().All(o => IsValidGenericValueType(o.GetType())));
            Values = Array.AsReadOnly(values);
            ValueTypes = Array.AsReadOnly(Values.Select(o => o.GetType().FullName).ToArray());
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Values);
        }
    }
}
