using Xunit;
using UnnamedStrategyGame.Serializers.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGameTests.TestHelpers;

namespace UnnamedStrategyGame.Serializers.JsonConverters.Tests
{
    public class XTypeJsonConverterTests
    {
        public XTypeJsonConverterTests()
        {
            Preloader.Preload();
        }

        [Fact()]
        public void CanConvertTest()
        {
            var conveter = new XTypeJsonConverter();
            Assert.True(conveter.CanConvert(typeof(BaseType)));
            Assert.True(conveter.CanConvert(typeof(UnitType)));
            Assert.True(conveter.CanConvert(typeof(Game.UnitTypes.Infantry)));
            Assert.False(conveter.CanConvert(typeof(string)));
            Assert.False(conveter.CanConvert(typeof(object)));
        }

        [Fact()]
        public void ReadWriteJsonTest()
        {
            var s = new JsonSerializer();

            SerializerCrossChecks.Check(Game.UnitTypes.Infantry.Instance, typeof(UnitType), s);
            SerializerCrossChecks.Check(Game.UnitTypes.ReconCar.Instance, typeof(UnitType), s);
            SerializerCrossChecks.Check(Game.TerrainTypes.City.Instance, typeof(TerrainType), s);
            SerializerCrossChecks.Check(Game.MovementTypes.Boots.Instance, typeof(MovementType), s);
            SerializerCrossChecks.Check(Game.ActionTypes.ForUnits.Move.Instance, typeof(ActionType), s);

        }

        [Fact()]
        public void WriteJsonTest_InvalidType()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                new XTypeJsonConverter().WriteJson(null, new object(), null);
            });
        }

        [Fact]
        public void GetInstanceFromTypeString_InvalidType()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                XTypeJsonConverter.GetInstanceFromTypeString(typeof(InvalidType), "nothing");
            });
        }

        public class InvalidType : BaseType
        {
            public InvalidType() : base("invalid_type") { }
        }
    }
}