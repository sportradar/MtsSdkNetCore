/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Builders;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;
using Sportradar.MTS.SDK.Test.Helpers;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;

namespace Sportradar.MTS.SDK.Test.Entities
{
    [TestClass]
    public class TicketAckTest
    {
        private IBuilderFactory _builderFactory;

        [TestInitialize]
        public void Init()
        {
            _builderFactory = new BuilderFactoryHelper().BuilderFactory;
            Assert.IsNotNull(_builderFactory);
        }

        [TestMethod]
        public void BuildTicketAckTest()
        {
            var ticket = new TicketAck("ticket-" + SR.I1000P, SR.I1000, TicketAckStatus.Accepted, 100, "message");

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.AreEqual(TicketHelper.MtsTicketVersion, ticket.Version);
            Assert.IsTrue(!string.IsNullOrEmpty(ticket.TicketId));
        }

        [TestMethod]
        public void BuildTicketAckFromTicketTest()
        {
            var ticketBuilder = _builderFactory.CreateTicketBuilder();
            var senderBuilder = _builderFactory.CreateSenderBuilder().SetBookmakerId(9985).SetLimitId(10).SetCurrency("EUR").SetSenderChannel(SenderChannel.Terminal);

            var ticket = ticketBuilder
                .SetTicketId("ticket-" + new Random().Next(10000)).SetOddsChange(OddsChangeType.Any)
                .SetSender(senderBuilder.SetEndCustomer(
                                _builderFactory.CreateEndCustomerBuilder().SetId("customer-client-" + SR.I1000).SetConfidence(12039203).SetLanguageId("en").Build())
                           .SetShopId(SR.S1000)
                           .SetTerminalId(SR.S1000)
                           .Build())
                .AddBet(new BuilderFactoryHelper().BuilderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                        .Build())
                .BuildTicket();

            var ticketAck = new TicketAck(ticket, TicketAckStatus.Accepted, 100, "message");

            Assert.IsNotNull(ticketAck);
            Assert.IsTrue(ticketAck.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.AreEqual(TicketHelper.MtsTicketVersion, ticketAck.Version);
            Assert.IsTrue(!string.IsNullOrEmpty(ticketAck.TicketId));
            Assert.AreEqual(ticketAck.TicketId, ticket.TicketId);
            Assert.AreEqual(ticket.Sender.BookmakerId, ticketAck.BookmakerId);
        }

        [TestMethod]
        public void BuildTicketAckAcceptanceTest()
        {
            var ticket = new TicketAck("ticket-" + SR.I1000P, SR.I1000, TicketAckStatus.Rejected, 100, "message");

            Assert.IsNotNull(ticket);
            Assert.AreEqual(ticket.TicketStatus, TicketAckStatus.Rejected);
        }

        [TestMethod]
        public void BuildTicketCancelAckAcceptanceTest()
        {
            var ticket = new TicketCancelAck("ticket-" + SR.I1000P, SR.I1000, TicketCancelAckStatus.NotCancelled, 100, "message");

            Assert.IsNotNull(ticket);
            Assert.AreEqual(ticket.TicketCancelStatus, TicketCancelAckStatus.NotCancelled);
        }
    }
}
