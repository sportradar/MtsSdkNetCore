/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Test.Helpers;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;

namespace Sportradar.MTS.SDK.Test.Builders
{
    [TestClass]
    public class TicketBuilderTest
    {
        private IBuilderFactory _builderFactory;
        private ITicketBuilder _ticketBuilder;
        private ISenderBuilder _senderBuilder;
        private ISender _sender;

        [TestInitialize]
        public void Init()
        {
            _builderFactory = new BuilderFactoryHelper().BuilderFactory;
            _ticketBuilder = _builderFactory.CreateTicketBuilder();
            _senderBuilder = _builderFactory.CreateSenderBuilder().SetBookmakerId(9985).SetLimitId(90).SetCurrency("EUR").SetSenderChannel(SenderChannel.Internet);
            _sender = _senderBuilder.SetEndCustomer(_builderFactory.CreateEndCustomerBuilder()
                                                        .SetId("customer-client-" + SR.I1000)
                                                        .SetConfidence(12039203)
                                                        .SetIp(IPAddress.Loopback)
                                                        .SetLanguageId("en")
                                                        .SetDeviceId("123")
                                                        .Build())
                                            .Build();
        }

        [TestMethod]
        public void BuildBaseTicketTest()
        {
            var ticket = _ticketBuilder
                        .SetTicketId("ticket-" + SR.S1000)
                        .SetOddsChange(OddsChangeType.Any)
                        .SetSender(_sender)
                        .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000)
                                               .SetStake(92343, StakeType.Total)
                                               .AddSelection(_builderFactory
                                                            .CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000)
                                                            .SetBanker(true).Build())
                                               .Build())
                        .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.AreEqual(ticket.Version, TicketHelper.MtsTicketVersion);
            Assert.IsTrue(!string.IsNullOrEmpty(ticket.TicketId));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoBetTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoSelectionTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total).Build())
                .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoSenderTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        public void BuildMultiBetTicketTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000P).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                        .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000P).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(false).Build())
                        .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000P).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                        .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000P).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                        .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000P).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Bets.Count() == 5);

            var previousBetId = string.Empty;
            long previousBetBonus = 0;
            foreach (var ticketBet in ticket.Bets)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ticketBet.Id));
                Assert.IsTrue(ticketBet.Bonus.Value > 0);
                Assert.AreNotEqual(previousBetId, ticketBet.Id);
                Assert.AreNotEqual(previousBetBonus, ticketBet.Bonus.Value);
                previousBetId = ticketBet.Id;
                previousBetBonus = ticketBet.Bonus.Value;
            }
            Assert.AreEqual(ticket.Bets.ToList()[3].Stake.Type, StakeType.Total);
            Assert.IsTrue(!string.IsNullOrEmpty(ticket.TicketId));
        }

        [TestMethod]
        public void BuildMultiSelectionTicketTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            var bet = ticket.Bets.First();

            Debug.Assert(bet != null, "bet != null");
            Assert.AreEqual(5, bet.Selections.Count());

            var selId = string.Empty;
            var eventId = "0";
            int? odds = 0;
            foreach (var sel in bet.Selections)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(sel.Id));
                Assert.IsTrue(!string.IsNullOrEmpty(sel.EventId));
                Assert.IsTrue(sel.Odds > 0);
                Assert.AreNotEqual(selId, sel.Id);
                Assert.AreNotEqual(eventId, sel.EventId);
                Assert.AreNotEqual(odds, sel.Odds);
                selId = sel.Id;
                eventId = sel.EventId;
                odds = sel.Odds;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildBetWithSameSelectionDifferentOddsTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(false).Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(26000).SetBanker(false).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildBetWithSameSelectionDifferentBankerTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(false).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        public void BuildMultiBetWithSameSelectionDifferentOddsTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(false).Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(26000).SetBanker(false).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public void BuildMultiBetWithSameSelectionDifferentOddsAndSameBankerTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(26000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public void BuildMultiBetWithSameSelectionDifferentBankerTest()
        {
            var ticket = _ticketBuilder
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(false).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
        }
    }
}