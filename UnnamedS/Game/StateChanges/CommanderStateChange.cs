using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.StateChanges
{
    public class CommanderStateChange : UpdateStateChange
    {
        public int CommanderID { get; }
        public Cause ChangeCause { get; }
        public CommanderStateChange(int commanderID, IDictionary<string, object> updatedProperties, Cause cause = Cause.Changed) : base(updatedProperties)
        {
            CommanderID = commanderID;
            ChangeCause = cause;
        }

        public enum Cause { Added, Removed, Changed }
    }
}
