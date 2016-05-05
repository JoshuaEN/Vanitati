using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.StateChanges;

namespace UnnamedStrategyGame.Network.MessageWrappers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class StartGameCallWrapper : CallMessageWrapper
    {
        public BattleGameState.Fields ChangeInfo { get; }

        public StartGameCallWrapper(BattleGameState.Fields changeInfo) : base(true)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);
            ChangeInfo = changeInfo;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.StartGame(ChangeInfo);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != ChangeInfo);
        }
    }
}
