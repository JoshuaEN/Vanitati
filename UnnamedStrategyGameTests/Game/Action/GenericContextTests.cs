using Xunit;
using UnnamedStrategyGame.Game.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Action.Tests
{
    public class GenericContextTests
    {
        public GenericContextTests()
        {
            Preloader.Preload();
        }

        [Fact()]
        public void IsValidGenericValueTypeTest()
        {
            Assert.True(TargetContext.IsValidGenericValueType(typeof(UnitType)));
            Assert.True(TargetContext.IsValidGenericValueType(typeof(UnitTypes.Infantry)));
            Assert.False(TargetContext.IsValidGenericValueType(typeof(object)));
        }

        [Fact()]
        public void GenericContextTest()
        {
            new GenericContext(UnitTypes.Infantry.Instance);

            Assert.Throws<ArgumentException>(() =>
            {
                new GenericContext(new object());
            });
        }
    }
}