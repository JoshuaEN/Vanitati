using Xunit;
using UnnamedStrategyGame.Serializers.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGameTests.TestHelpers;

namespace UnnamedStrategyGame.Serializers.JsonConverters.Tests
{
    public class ContextConverterTests
    {
        [Fact()]
        public void CanConvertTest()
        {
            var converter = new ContextConverter();
            Assert.True(converter.CanConvert(typeof(Context)));
            Assert.False(converter.CanConvert(typeof(UnitContext)));
            Assert.False(converter.CanConvert(typeof(CommanderContext)));
            Assert.False(converter.CanConvert(typeof(object)));
            Assert.False(converter.CanConvert(typeof(ActionContext)));
        }

        [Fact()]
        public void ReadJsonTest()
        {
            var s = new JsonSerializer();

            SerializerCrossChecks.Check(new UnitContext(new Location(1, 1)), typeof(Context), s);
            SerializerCrossChecks.Check(new TerrainContext(new Location(1, 1)), typeof(Context), s);
            SerializerCrossChecks.Check(new CommanderContext(5), typeof(Context), s);
            SerializerCrossChecks.Check(new GameContext(), typeof(Context), s);
        }

        [Fact()]
        public void ReadJsonTest_Invalid()
        {
            var s = new JsonSerializer();

            SerializerCrossChecks.CheckNot(new UnitContext(new Location(1, 1)), new UnitContext(new Location(1, 0)), typeof(Context), s);
            SerializerCrossChecks.CheckNot(new TerrainContext(new Location(1, 1)), new TerrainContext(new Location(0, 1)), typeof(Context), s);
            SerializerCrossChecks.CheckNot(new CommanderContext(5), new CommanderContext(10), typeof(Context), s);
        }

        [Fact()]
        public void ReadJsonTest_Tampered()
        {
            var s = new JsonSerializer();

            var context = new OtherContext();
            var sres = s.Serialize(context);
            var tsres = sres.Replace(context.Type.ToString(), typeof(object).ToString());

            Assert.Throws<ArgumentException>(() =>
            {
                s.Deserialize<Context>(tsres);
            });
        }

    }
}