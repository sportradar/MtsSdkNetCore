/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;

namespace Sportradar.MTS.SDK.Test.Entities
{
    [TestClass]
    public class EntitiesBuilderTest
    {
        [TestMethod]
        public void CreateSelectionTest()
        {
            var item = new Selection(SR.S1000, SR.S1000, 123456, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void CreateSelectionIdLengthTest()
        {
            var item = new Selection(SR.S1000, SR.SL(1000), 123456, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CreateSelectionWrongEventIdTest()
        {
            var item = new Selection(string.Empty, SR.S1000, SR.I1000, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CreateSelectionWrongIdTest()
        {
            var item = new Selection(SR.S1000, string.Empty, SR.I1000, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CreateSelectionWrongId2Test()
        {
            var item = new Selection(SR.S1000, SR.SL(1001), 123456, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CreateSelectionZeroOddsTest()
        {
            var item = new Selection(SR.S1000, SR.S1000, 0, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CreateSelectionWrongOddsTest()
        {
            var item = new Selection(SR.S1000, SR.S1000, SR.I1000, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CreateSelectionWrongOddsOnGetTest()
        {
            var item = new Selection(SR.S1000, SR.S1000, 9000, SR.B);

            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void CreateStakeTest()
        {
            var item = new Stake(SR.I1000, StakeType.Unit);

            Assert.IsNotNull(item);
        }

        [Ignore("Promobet")]
        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CreateStakeWrongIdTest()
        {
            var item = new Stake(0, StakeType.Unit);

            Assert.IsNotNull(item);
        }
    }
}