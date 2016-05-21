using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [ContractClass(typeof(ContractClassForCallMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class CallMessageWrapper : MessageWrapper
    {
        public abstract bool RequiresHost { get; }

        public abstract void Call(LocalGameLogic logic);
    }

    [ContractClassFor(typeof(CallMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForCallMessageWrapper : CallMessageWrapper
    {
        public override void Call(LocalGameLogic logic)
        {
            Contract.Requires<ArgumentNullException>(null != logic);
        }
    }
}
