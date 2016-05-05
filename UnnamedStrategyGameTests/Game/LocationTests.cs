using Xunit;
using UnnamedStrategyGame.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnnamedStrategyGame.Game.Tests
{
    public class LocationTests
    {

        [Fact()]
        public void EqualsTest()
        {
            var a = new Location(0, 0);
            var a2 = new Location(0, 0);
            var b = new Location(0, 1);
            var b2 = new Location(0, 1);
            var c = new Location(1, 0);
            var c2 = new Location(1, 0);
            var d = new Location(1, 1);

#pragma warning disable CS1718 // Comparison made to same variable
            Assert.True(a == a);
#pragma warning restore CS1718 // Comparison made to same variable
            Assert.True(a.Equals(a));
            Assert.True(a == a2);
            Assert.True(a2 == a);
            Assert.True(a.Equals(a2));
            Assert.True(a2.Equals(a));
            Assert.True(b == b2);
            Assert.True(b2 == b);
            Assert.True(b.Equals(b2));
            Assert.True(b2.Equals(b));
            Assert.True(c == c2);
            Assert.True(c2 == c);
            Assert.True(c.Equals(c2));
            Assert.True(c2.Equals(c));

            Assert.False(a == b);
            Assert.False(a == c);
            Assert.False(a == d);

            Assert.False(b == a);
            Assert.False(c == a);
            Assert.False(d == a);

            Assert.False(b == c);
            Assert.False(b == d);

            Assert.False(d == b);
            Assert.False(c == b);

            Assert.False(c == d);
            Assert.False(d == c);

            var o = new object();
            Assert.False(a == null);
            Assert.False(null == a);
            Assert.False(a.Equals((object)null));
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
            Assert.False(a == o);
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            Assert.False(o == a);
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
            Assert.False(a.Equals(new object()));
        }

        [Fact]
        public void ToStringTest()
        {
            new Location().ToString();
        }
    }
}