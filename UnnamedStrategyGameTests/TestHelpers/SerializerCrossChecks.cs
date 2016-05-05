using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Serializers;
using Xunit;

namespace UnnamedStrategyGameTests.TestHelpers
{
    public static class SerializerCrossChecks
    {
        public static void Check(object expected, Type type, BaseSerializer serializer)
        {
            string sres = serializer.Serialize(expected);
            object dres = serializer.Deserialize(sres, type);

            Assert.Equal(expected.GetType(), dres.GetType());
            Assert.Equal(sres, serializer.Serialize(dres));
        }

        public static void CheckNot(object input, object not, Type type, BaseSerializer serializer)
        {
            string sres = serializer.Serialize(input);
            string snotres = serializer.Serialize(not);
            object dres = serializer.Deserialize(sres, type);
            object dnotres = serializer.Deserialize(snotres, type);

            Assert.NotEqual(sres, snotres);
            Assert.NotEqual(sres, serializer.Serialize(dnotres));
            Assert.NotEqual(serializer.Serialize(dres), serializer.Serialize(dnotres));
        }
    }
}
