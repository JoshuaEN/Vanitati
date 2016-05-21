using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CommanderContext : SourceContext
    {
        public int CommanderID { get; }

        public override ActionType.Category ActionCategory
        {
            get
            {
                return ActionType.Category.Commander;
            }
        }

        public CommanderContext(int commanderID)
        {
            CommanderID = commanderID;
        }
    }
}
