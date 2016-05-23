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

        public GenericContext(string[] value) : this(new object[] { value }) { }

        public GenericContext(params object[] values)
        {
            Contract.Requires<ArgumentNullException>(null != values);

            foreach(var v in values)
            {
                if(IsValidGenericValueType(v.GetType()) == false)
                {
                    throw new ArgumentException();
                }
            }
            Values = Array.AsReadOnly(values);
            ValueTypes = Array.AsReadOnly(Values.Select(o => {

                var t = o.GetType();

                if (t.IsGenericType)
                    throw new ArgumentException("Generic Types not Supported");

                return t.AssemblyQualifiedName;
            }).ToArray());
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Values);
        }
    }
}
