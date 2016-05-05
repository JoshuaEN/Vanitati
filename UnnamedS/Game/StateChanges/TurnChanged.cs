using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TurnChanged : StateChange
    {
        public int PreviousTurnID { get; }
        public int NewTurnID { get; }
        public int PreviousCommanderID { get; }
        public int NextCommanderID { get; }
        public Cause ChangeCause { get; }

        public TurnChanged(int previousTurnID, int newTurnID, int previousCommanderID, int nextCommanderID, Cause changeCause)
        {
            PreviousTurnID = previousTurnID;
            NewTurnID = newTurnID;
            PreviousCommanderID = previousCommanderID;
            NextCommanderID = nextCommanderID;
            ChangeCause = changeCause;
        }

        public enum Cause { GameStart, TurnEnded }
    }
}
