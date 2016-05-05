using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Event;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class OnTurnChangedNotifyWrapper : NotifyMessageWrapper
    {
        public TurnChangedEventArgs Args { get; }

        public OnTurnChangedNotifyWrapper(TurnChangedEventArgs args)
        {
            Contract.Requires<ArgumentNullException>(null != args);
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnTurnChanged(this, Args);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Args);
        }
    }
}
