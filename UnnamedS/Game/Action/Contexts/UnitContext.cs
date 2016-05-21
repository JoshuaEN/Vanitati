using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UnitContext : TileContext
    {
        public override ActionType.Category ActionCategory
        {
            get
            {
                return ActionType.Category.Unit;
            }
        }

        public UnitContext(Location location) : base(location) { }
    }
}
