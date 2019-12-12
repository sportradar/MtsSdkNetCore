/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Utils;
using Sportradar.MTS.SDK.Test.Helpers;
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Sportradar.MTS.SDK.Test.Mapping
{
    [TestClass]
    public class TicketSerializerTest
    {
        private IBuilderFactory _builderFactory;

        [TestInitialize]
        public void Init()
        {
            _builderFactory = new BuilderFactoryHelper().BuilderFactory;
        }

        [TestMethod]
        public void SerializeTicketTest()
        {
            var ticket = TicketBuilderHelper.GetTicket();
            var serialized = JsonConvert.SerializeObject(ticket);
            var deserializedTicket = JsonUtils.Deserialize<ITicket>(serialized);
            var serializedAgain = JsonUtils.Serialize(deserializedTicket);

            Assert.AreEqual(serialized, serializedAgain);
        }

        [TestMethod]
        public void SerializeTicketCancelTest()
        {
            var ticket = TicketBuilderHelper.GetTicketCancel();
            var serialized = JsonConvert.SerializeObject(ticket);
            var deserializedTicket = JsonUtils.Deserialize<ITicketCancel>(serialized);
            var serializedAgain = JsonUtils.Serialize(deserializedTicket);

            Assert.AreEqual(serialized, serializedAgain);
        }

        [TestMethod]
        public void SerializeTicketCashoutTest()
        {
            var ticket = TicketBuilderHelper.GetTicketCashout();
            var serialized = JsonConvert.SerializeObject(ticket);
            var deserializedTicket = JsonUtils.Deserialize<ITicketCashout>(serialized);
            var serializedAgain = JsonUtils.Serialize(deserializedTicket);

            Assert.AreEqual(serialized, serializedAgain);
        }

        [TestMethod]
        public void SerializeTicketReofferCancelTest()
        {
            var ticket = TicketBuilderHelper.GetTicketReofferCancel();
            var serialized = JsonConvert.SerializeObject(ticket);
            var deserializedTicket = JsonUtils.Deserialize<ITicketReofferCancel>(serialized);
            var serializedAgain = JsonUtils.Serialize(deserializedTicket);

            Assert.AreEqual(serialized, serializedAgain);
        }

        [TestMethod]
        public void SerializeTicketNonSrSettleTest()
        {
            var ticket = TicketBuilderHelper.GetTicketNonSrSettle();
            var serialized = JsonConvert.SerializeObject(ticket);
            var deserializedTicket = JsonUtils.Deserialize<ITicketNonSrSettle>(serialized);
            var serializedAgain = JsonUtils.Serialize(deserializedTicket);

            Assert.AreEqual(serialized, serializedAgain);
        }
    }
}