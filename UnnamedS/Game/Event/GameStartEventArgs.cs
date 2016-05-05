using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Event
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameStartEventArgs : EventArgs
    {
        public BattleGameState.Fields ChangeInfo { get; }

        public GameStartEventArgs(BattleGameState.Fields changeInfo)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);

            ChangeInfo = changeInfo;
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != ChangeInfo);
        }
    }
}
