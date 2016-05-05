using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TurnEnd : StateChange
    {
        public int CommanderID { get; }

        public TurnEnd(int commanderID)
        {
            CommanderID = commanderID;
        }
    }
}
