using Xunit;
using UnnamedStrategyGame.Serializers.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game.Action;
using UnnamedStrategyGameTests.TestHelpers;
using System.Reflection;

namespace UnnamedStrategyGame.Serializers.JsonConverters.Tests
{
    public class GenericContextConverterTests
    {
        public GenericContextConverterTests()
        {
            Game.Preloader.Preload();

            // Ref: http://stackoverflow.com/a/21888521
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, Assembly.GetAssembly(typeof(Game.UnitType)));

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }

        [Fact()]
        public void CanConvertTest()
        {
            var converter = new GenericContextConverter();
            Assert.True(converter.CanConvert(typeof(GenericContext)));
            Assert.False(converter.CanConvert(typeof(Context)));
            Assert.False(converter.CanConvert(typeof(OtherContext)));
            Assert.False(converter.CanConvert(typeof(object)));
        }

        [Fact()]
        public void ReadJsonTest()
        {
            var s = new JsonSerializer();

            var context = new GenericContext(Game.UnitTypes.Infantry.Instance);

            var sres = s.Serialize(context);
            var dres = s.Deserialize<GenericContext>(sres);

            Assert.Equal(context.Value.GetType(), dres.Value.GetType());
            Assert.Equal(context.Value, dres.Value);
        }

        [Fact()]
        public void ReadJsonTest_Tampered()
        {
            // Ref: http://stackoverflow.com/a/21888521
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, Assembly.GetAssembly(typeof(object)));

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
            var s = new JsonSerializer();

            var context = new GenericContext(Game.UnitTypes.Infantry.Instance);
            var sres = s.Serialize(context);
            var tsres = sres.Replace(context.ValueType, typeof(object).FullName);
            object pez;
            Assert.Throws<ArgumentException>(() =>
            {
                pez = s.Deserialize<GenericContext>(tsres);

                pez.ToString();
            });

            tsres = sres.Replace(context.ValueType, "ThisIsNotAnActualTypeThatCanBeFound");

            Assert.Throws<ArgumentException>(() =>
            {
                s.Deserialize<GenericContext>(tsres);
            });
        }
    }
}