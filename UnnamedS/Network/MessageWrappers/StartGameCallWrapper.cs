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
        public override bool RequiresHost { get; } = true;
        public BattleGameState.Fields ChangeInfo { get; }
        public BattleGameState.StartMode StartMode { get; }

        public StartGameCallWrapper(BattleGameState.Fields changeInfo, BattleGameState.StartMode startMode)
        {
            Contract.Requires<ArgumentNullException>(null != changeInfo);
            ChangeInfo = changeInfo;
            StartMode = startMode;
        }

        public override void Call(LocalGameLogic logic)
        {
            logic.StartGame(ChangeInfo, StartMode);
        }

        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(null != ChangeInfo);
        }
    }
}
