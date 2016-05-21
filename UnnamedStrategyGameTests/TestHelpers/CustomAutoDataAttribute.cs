using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGame.Game.Action;

namespace UnnamedStrategyGameTests.TestHelpers
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(GetFixture()) { }

        private static Fixture GetFixture()
        {
            var f = new Fixture();
            f.Register<ActionType>(() => UnnamedStrategyGame.Game.ActionTypes.ForUnits.Move.Instance);
            f.Register<SourceContext>(() => new FakeConcrete.FakeSourceContext());
            f.Register<TargetContext>(() => new FakeConcrete.FakeTargetContext());
            return f;
        }
    }
}
