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
    public class OnGameStartNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.GameStartEventArgs Args { get; }

        public OnGameStartNotifyWrapper(Game.Event.GameStartEventArgs args)
        {
            Contract.Requires<ArgumentNullException>(null != args);
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnGameStart(this, Args);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Args);
        }
    }
}
