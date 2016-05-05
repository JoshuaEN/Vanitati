using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGameTests.TestHelpers;

namespace UnnamedStrategyGame.Game.Properties.PropertyContainerHelper.Tests
{
    public class HelperTests
    {
        [Theory, CustomAutoData]
        public void GetPropertiesTest(HelperTestClass testKlass)
        {
            var props = new PropertyContainer.Helper<HelperTestClass>().GetProperties(testKlass);
            var keys = props.Keys;
            var values = props.Values;

            Assert.Contains("Get", keys);
            Assert.Equal(testKlass.Get, props["Get"]);

            Assert.Contains("GetPrivateSet", keys);
            Assert.Equal(testKlass.GetPrivateSet, props["GetPrivateSet"]);

            Assert.Contains("GetSet", keys);
            Assert.Equal(testKlass.GetSet, props["GetSet"]);

            Assert.Equal(3, props.Count);
        }

        [Theory, CustomAutoData]
        public void GetWriteablePropertiesTest(HelperTestClass testKlass)
        {
            var props = new PropertyContainer.Helper<HelperTestClass>().GetWriteableProperties(testKlass);
            var keys = props.Keys;
            var values = props.Values;

            Assert.Contains("GetPrivateSet", keys);
            Assert.Equal(testKlass.GetPrivateSet, props["GetPrivateSet"]);

            Assert.Contains("GetSet", keys);
            Assert.Equal(testKlass.GetSet, props["GetSet"]);

            Assert.Equal(2, props.Count);
        }

        [Theory, CustomAutoData]
        public void SetPropertiesTest(HelperTestClass testKlass)
        {
            var helper = new PropertyContainer.Helper<HelperTestClass>();

            var priorValues = helper.GetWriteableProperties(testKlass);

            var values = priorValues.ToDictionary(kp => kp.Key, kp => kp.Value);

            foreach(var key in values.Keys.ToList())
            {
                values[key] = ((int)values[key]) + 1;
            }

            helper.SetProperties(testKlass, values);

            var valuesAfter = helper.GetWriteableProperties(testKlass);

            Assert.Equal(2, values.Count);
            Assert.Equal(priorValues.Count, values.Count);
            Assert.Equal(values.Count, valuesAfter.Count);

            foreach(var key in values.Keys)
            {
                var before = priorValues[key];
                var asSet = values[key];
                var after = valuesAfter[key];

                Assert.NotEqual(before, after);
                Assert.Equal((int)before + 1, after);
                Assert.Equal(asSet, after);
            }
        }

        [Theory, CustomAutoData]
        public void SetPropertiesTest_ReadOnlyProperty(HelperTestClass testKlass)
        {
            var helper = new PropertyContainer.Helper<HelperTestClass>();

            Assert.Throws<Exceptions.PropertyReadOnlyException>(() =>
            {
                helper.SetProperties(testKlass, new Dictionary<string, object>() { { "Get", 5 } });
            });

        }

        [Theory, CustomAutoData]
        public void SetPropertiesTest_NonExistantProperty(HelperTestClass testKlass)
        {
            var helper = new PropertyContainer.Helper<HelperTestClass>();

            Assert.Throws<Exceptions.UnknownPropertyException>(() =>
            {
                helper.SetProperties(testKlass, new Dictionary<string, object>() { { "DoesNotExist", 5 } });
            });

        }

        [Theory, CustomAutoData]
        public void SetPropertiesTest_IncompatableTypesProperty(HelperTestClass testKlass)
        {
            var helper = new PropertyContainer.Helper<HelperTestClass>();

            Assert.Throws<Exceptions.IncompatiblePropertyException>(() =>
            {
                helper.SetProperties(testKlass, new Dictionary<string, object>() { { "GetSet", "echo" } });
            });

        }

        public class HelperTestClass
        {
            public int Get { get; }
            public int GetPrivateSet { get; private set; }

            private int _set;
            public int Set { set { _set = value; } }

            public int GetSet { get; set; }
        }
    }
}