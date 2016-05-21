﻿using Xunit;
using UnnamedStrategyGame.Game.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnnamedStrategyGameTests.TestHelpers;
using UnnamedStrategyGameTests.TestHelpers.FakeConcrete;

namespace UnnamedStrategyGame.Game.Action.Tests
{
    public class ActionContextTests
    {

        [Fact]
        public void ConvertToSpecificContextTest_LoadBoth()
        {
            UnitContext source = new UnitContext(new Location());
            TargetContext target = new GenericContext(new Location());

            var ac = new ActionContext(default(int), ActionContext.TriggerAutoDetermineMode.ManuallyByUser, source, target);

            UnitContext outSource;
            TargetContext outTarget;

            ac.ConvertToSpecificContext(TargetContextBase.Load.Source | TargetContextBase.Load.Target, out outSource, out outTarget);

            Assert.Equal(source, outSource);
            Assert.Equal(target, outTarget);
        }

        [Fact]
        public void ConvertToSpecificContextTest_LoadSource()
        {
            UnitContext source = new UnitContext(new Location());
            FakeTargetContext target = new FakeTargetContext();

            var ac = new ActionContext(default(int), ActionContext.TriggerAutoDetermineMode.ManuallyByUser, source, target);

            UnitContext outSource;
            TerrainContext outTarget;

            ac.ConvertToSpecificContext(TargetContextBase.Load.Source, out outSource, out outTarget);

            Assert.Equal(source, outSource);
            Assert.Equal(null, outTarget);
        }

        [Fact]
        public void ConvertToSpecificContextTest_LoadTarget()
        {
            FakeSourceContext source = new FakeSourceContext();
            TargetContext target = new GenericContext(new Location());

            var ac = new ActionContext(default(int), ActionContext.TriggerAutoDetermineMode.ManuallyByUser, source, target);

            UnitContext outSource;
            TargetContext outTarget;

            ac.ConvertToSpecificContext(TargetContextBase.Load.Target, out outSource, out outTarget);

            Assert.Equal(null, outSource);
            Assert.Equal(target, outTarget);
        }

        [Fact]
        public void ConvertToSpecificContextTest_IncompatableSourceType()
        {
            UnitContext source = new UnitContext(new Location());
            TargetContext target = new GenericContext(new Location());

            var ac = new ActionContext(default(int), ActionContext.TriggerAutoDetermineMode.ManuallyByUser, source, target);

            FakeSourceContext outSource;
            TargetContext outTarget;

            Assert.Throws<ArgumentException>(() =>
            {
                ac.ConvertToSpecificContext(TargetContextBase.Load.Source | TargetContextBase.Load.Target, out outSource, out outTarget);
            });
        }

        [Fact]
        public void ConvertToSpecificContextTest_IncompatableTargetType()
        {
            UnitContext source = new UnitContext(new Location());
            TargetContext target = new GenericContext(new Location());

            var ac = new ActionContext(default(int), ActionContext.TriggerAutoDetermineMode.ManuallyByUser, source, target);

            UnitContext outSource;
            FakeTargetContext outTarget;

            Assert.Throws<ArgumentException>(() =>
            {
                ac.ConvertToSpecificContext(TargetContextBase.Load.Source | TargetContextBase.Load.Target, out outSource, out outTarget);
            });
        }

        [Fact]
        public void ConvertToSpecificContextTest_NullTargetContext()
        {
            UnitContext source = new UnitContext(new Location());
            NullContext target = new NullContext();

            var ac = new ActionContext(default(int), ActionContext.TriggerAutoDetermineMode.ManuallyByUser, source, target);

            UnitContext outSource;
            NullContext outTarget;

            Assert.Throws<ArgumentException>(() =>
            {
                ac.ConvertToSpecificContext(TargetContextBase.Load.Source | TargetContextBase.Load.Target, out outSource, out outTarget);
            });
        }
    }
}