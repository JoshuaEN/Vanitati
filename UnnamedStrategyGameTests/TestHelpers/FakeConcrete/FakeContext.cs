using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGameTests.TestHelpers.FakeConcrete
{
    public class FakeSourceContext : SourceContext
    {
        public override ActionType.Category ActionCategory
        {
            get
            {
                return ActionType.Category.Commander;
            }
        }
    }

    public class FakeTargetContext : GenericContext
    {

    }
}
