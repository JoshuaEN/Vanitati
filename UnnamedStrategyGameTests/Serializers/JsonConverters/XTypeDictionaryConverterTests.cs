using Xunit;
using UnnamedStrategyGame.Serializers.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Game;
using UnnamedStrategyGameTests.TestHelpers;
using System.Collections;

namespace UnnamedStrategyGame.Serializers.JsonConverters.Tests
{
    public class XTypeDictionaryConverterTests
    {
        public XTypeDictionaryConverterTests()
        {
            Preloader.Preload();
        }

        [Fact()]
        public void CanConvertTest()
        {
            var conveter = new XTypeDictionaryConverter();
            Assert.True(conveter.CanConvert(typeof(Dictionary<BaseType, string>)));
            Assert.True(conveter.CanConvert(typeof(Dictionary<UnitType, string>)));
            Assert.True(conveter.CanConvert(typeof(IDictionary<UnitType, string>)));
            Assert.True(conveter.CanConvert(typeof(Dictionary<Game.UnitTypes.Infantry, ContextBoundObject>)));
            Assert.True(conveter.CanConvert(typeof(FakeDictionary<TerrainType, object>)));
            Assert.False(conveter.CanConvert(typeof(Dictionary<string, BaseType>)));
            Assert.False(conveter.CanConvert(typeof(Dictionary<object, string>)));
            Assert.False(conveter.CanConvert(typeof(object)));
        }

        [Fact()]
        public void ReadWriteJsonTest()
        {
            var s = new JsonSerializer();

            SerializerCrossChecks.Check(new Dictionary<UnitType, int>()
            {
                {Game.UnitTypes.Infantry.Instance, 5 },
                {Game.UnitTypes.ReconCar.Instance, 10 }
            }, typeof(Dictionary<UnitType, int>), s);
        }

        class FakeDictionary<A,B> : System.Collections.IDictionary
        {
            public object this[object key]
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string this[BaseType key]
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsFixedSize
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsSynchronized
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<BaseType> Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public object SyncRoot
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public ICollection<string> Values
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            ICollection IDictionary.Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            ICollection IDictionary.Values
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public void Add(KeyValuePair<BaseType, string> item)
            {
                throw new NotImplementedException();
            }

            public void Add(object key, object value)
            {
                throw new NotImplementedException();
            }

            public void Add(BaseType key, string value)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(object key)
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<BaseType, string> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(BaseType key)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<BaseType, string>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<BaseType, string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void Remove(object key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<BaseType, string> item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(BaseType key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(BaseType key, out string value)
            {
                throw new NotImplementedException();
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

    }
}