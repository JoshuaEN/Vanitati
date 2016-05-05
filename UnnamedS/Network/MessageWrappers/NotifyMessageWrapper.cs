using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [ContractClass(typeof(ContractClassForNotifyMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public abstract class NotifyMessageWrapper : MessageWrapper
    {
        public abstract void Notify(IUserLogic logic);
    }

    [ContractClassFor(typeof(NotifyMessageWrapper))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal abstract class ContractClassForNotifyMessageWrapper : NotifyMessageWrapper
    {
        public override void Notify(IUserLogic logic)
        {
            Contract.Requires<ArgumentNullException>(null != logic);
        }
    }
}
