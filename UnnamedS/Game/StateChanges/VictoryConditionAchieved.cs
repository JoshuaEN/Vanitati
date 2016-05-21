using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class VictoryConditionAchieved : StateChange
    {
        public int[] Winners { get; }
        public string WinConditionKey { get; }

        public VictoryConditionAchieved(int[] winners, string winConditionKey)
        {
            Contract.Requires<ArgumentNullException>(null != winners);
            Contract.Requires<ArgumentNullException>(null != winConditionKey);

            Winners = winners;
            WinConditionKey = winConditionKey;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != WinConditionKey);
            Contract.Invariant(null != Winners);
        }
    }
}
