using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnExceptionNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.ExceptionEventArgs Args { get; }

        public OnExceptionNotifyWrapper(Game.Event.ExceptionEventArgs args)
        {
            Contract.Requires<ArgumentNullException>(null != args);

            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnException(this, Args);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Args);
        }
    }
}
