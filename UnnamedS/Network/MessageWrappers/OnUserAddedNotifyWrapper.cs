using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class OnUserAddedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.UserAddedEventArgs Args { get; }

        public OnUserAddedNotifyWrapper(Game.Event.UserAddedEventArgs args)
        {
            Contract.Requires<ArgumentNullException>(null != args);
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnUserAdded(this, Args);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Args);
        }
    }
}
