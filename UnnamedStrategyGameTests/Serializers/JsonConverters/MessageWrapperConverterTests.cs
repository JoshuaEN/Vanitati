using Xunit;
using UnnamedStrategyGame.Serializers.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGame.Network.MessageWrappers;
using UnnamedStrategyGameTests.TestHelpers;

namespace UnnamedStrategyGame.Serializers.JsonConverters.Tests
{
    public class MessageWrapperConverterTests
    {
        [Fact()]
        public void CanConvertTest()
        {
            var converter = new MessageWrapperConverter();
            Assert.True(converter.CanConvert(typeof(MessageWrapper)));
            Assert.False(converter.CanConvert(typeof(CallMessageWrapper)));
            Assert.False(converter.CanConvert(typeof(NotifyMessageWrapper)));
            Assert.False(converter.CanConvert(typeof(object)));
        }

        [Fact()]
        public void ReadJsonTest()
        {
            var s = new JsonSerializer();

            SerializerCrossChecks.Check(new OnTurnChangedNotifyWrapper(new Game.Event.TurnChangedEventArgs(0, new Game.StateChanges.TurnChanged(0, 1, 0, 1, Game.StateChanges.TurnChanged.Cause.TurnEnded))), typeof(MessageWrapper), s);
        }

        [Fact()]
        public void ReadJsonTest_Invalid()
        {
            var s = new JsonSerializer();

            SerializerCrossChecks.CheckNot(
                new OnTurnChangedNotifyWrapper(new Game.Event.TurnChangedEventArgs(0, new Game.StateChanges.TurnChanged(0, 1, 0, 1, Game.StateChanges.TurnChanged.Cause.TurnEnded))),
                new OnTurnChangedNotifyWrapper(new Game.Event.TurnChangedEventArgs(1, new Game.StateChanges.TurnChanged(0, 1, 0, 1, Game.StateChanges.TurnChanged.Cause.TurnEnded))),
                typeof(MessageWrapper), s);
        }

        [Fact()]
        public void ReadJsonTest_Tampered()
        {
            var s = new JsonSerializer();

            var wrapper = new OnTurnChangedNotifyWrapper(new Game.Event.TurnChangedEventArgs(0, new Game.StateChanges.TurnChanged(0, 1, 0, 1, Game.StateChanges.TurnChanged.Cause.TurnEnded)));
            var sres = s.Serialize(wrapper);
            var tsres = sres.Replace(wrapper.Type, typeof(object).FullName);

            Assert.Throws<ArgumentException>(() =>
            {
                s.Deserialize<MessageWrapper>(tsres);
            });
        }
    }
}