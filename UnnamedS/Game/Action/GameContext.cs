using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GameContext : Context
    {
        public override bool CanBeTarget
        {
            get
            {
                return false;
            }
        }

        public override ActionType.Category ActionCategory
        {
            get
            {
                return ActionType.Category.Game;
            }
        }

        public override ActionType.TargetCategory ActionTargetCategory
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}
