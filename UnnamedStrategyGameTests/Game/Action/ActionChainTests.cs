using Xunit;
using UnnamedStrategyGame.Game.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture.Xunit2;
using UnnamedStrategyGameTests.TestHelpers;

namespace UnnamedStrategyGame.Game.Action.Tests
{
    public class ActionChainTests
    {
        [Theory, CustomAutoData]
        public void ActionChainTestListConstructor(List<ActionChain.Link> links)
        {
            Assert.Equal(links.Count, new ActionChain(links).Length);
        }

        [Theory, CustomAutoData]
        public void ActionChainTestParamConstructor(ActionChain.Link a, ActionChain.Link b, ActionChain.Link c)
        {
            var actionChain = new ActionChain(a, b, c);
            Assert.Equal(3, actionChain.Length);

            var chain = actionChain.GetActionsInfo(default(int));
            CompareLinkToActionInfo(a, chain[0]);
            CompareLinkToActionInfo(b, chain[1]);
            CompareLinkToActionInfo(c, chain[2]);
        }

        [Theory, CustomAutoData]
        public void AddActionTestViaLink(ActionChain actionChain, ActionChain.Link link)
        {
            actionChain.AddAction(link);
            CompareLinkToActionInfo(link, actionChain.GetActionsInfo(default(int))[0]);
        }

        [Theory, CustomAutoData]
        public void AddActionTest_ViaLink_Null(ActionChain actionChain)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                actionChain.AddAction(null);
            });
            
        }

        [Theory, CustomAutoData]
        public void AddActionTestViaParams(ActionChain actionChain, ActionType type, Context source, Context target)
        {
            actionChain.AddAction(type, source, target);
            Assert.Equal(1, actionChain.Length);

            var chain = actionChain.GetActionsInfo(default(int));
            Assert.Equal(type, chain[0].Type);
            Assert.Equal(source, chain[0].Context.Source);
            Assert.Equal(target, chain[0].Context.Target);
        }

        [Theory, CustomAutoData]
        public void GetActionsInfoTest(List<ActionChain.Link> links, int commanderID)
        {
            var actionChain = new ActionChain(links);

            var chain = actionChain.GetActionsInfo(commanderID);

            Assert.Equal(links.Count, chain.Count);

            for(var i = 0; i < links.Count; i++)
            {
                var expected = links[i];
                var actual = chain[i];

                CompareLinkToActionInfo(expected, actual);
                Assert.Equal(commanderID, actual.Context.TriggeredByCommanderID);
            }
        }

        [Theory, CustomAutoData]
        public void GetActionsTest(List<ActionChain.Link> links)
        {
            var actionChain = new ActionChain(links);

            var actions = actionChain.GetActions();

            Assert.Equal(links.Count, actions.Count);

            for (var i = 0; i < links.Count; i++)
            {
                var expected = links[i];
                var actual = actions[i];

                Assert.Equal(expected.Action, actual);
            }
        }

        private void CompareLinkToActionInfo(ActionChain.Link link, ActionInfo info)
        {
            Assert.Equal(link.Action, info.Type);
            Assert.Equal(link.Source, info.Context.Source);
            Assert.Equal(link.Target, info.Context.Target);
        }
    }
}