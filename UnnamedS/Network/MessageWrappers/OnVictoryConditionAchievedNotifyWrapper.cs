using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    public class OnVictoryConditionAchievedNotifyWrapper : NotifyMessageWrapper
    {
        public Game.Event.VictoryConditionAchievedEventArgs Args { get; }

        public OnVictoryConditionAchievedNotifyWrapper(Game.Event.VictoryConditionAchievedEventArgs args)
        {
            Contract.Requires<ArgumentNullException>(null != args);
            Args = args;
        }

        public override void Notify(IUserLogic logic)
        {
            logic.OnVictoryConditionAchieved(this, Args);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != Args);
        }
    }
}
