using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameContext : SourceContext
    {
        public override ActionType.Category ActionCategory { get; } = ActionType.Category.Game;
    }
}
