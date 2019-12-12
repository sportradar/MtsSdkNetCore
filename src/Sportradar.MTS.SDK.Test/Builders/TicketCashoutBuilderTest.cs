/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Builders;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;

namespace Sportradar.MTS.SDK.Test.Builders
{
    [TestClass]
    public class TicketCashoutBuilderTest
    {
        [TestMethod]
        public void BuildBaseTicketTest()
        {
            var tb = TicketCashoutBuilder.Create();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .SetCashoutStake(1000)
                           .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.AreEqual(TicketHelper.MtsTicketVersion, ticket.Version);
            Assert.IsTrue(TicketHelper.ValidateTicketId(ticket.TicketId));
            Assert.IsNull(ticket.CashoutPercent);
            Assert.IsNull(ticket.BetCashouts);
            Assert.IsNotNull(ticket.CashoutStake);
            Assert.AreEqual(1000, ticket.CashoutStake);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoCodeTest()
        {
            var tb = TicketCashoutBuilder.Create();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoBookmakerIdTest()
        {
            var tb = TicketCashoutBuilder.Create();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetCashoutStake(1000)
                           .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoTicketIdTest()
        {
            var tb = TicketCashoutBuilder.Create();
            var ticket = tb.SetBookmakerId(SR.I1000)
                           .SetCashoutStake(1000)
                           .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        public void BuildTicketPercentTest()
        {
            var tb = TicketCashoutBuilder.Create();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .SetCashoutStake(1000)
                           .SetCashoutPercent(2132)
                           .BuildTicket();
            var dto = new TicketCashoutMapper().Map(ticket);

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.IsNotNull(ticket.CashoutPercent);
            Assert.IsNull(ticket.BetCashouts);
            Assert.IsNotNull(ticket.CashoutStake);
            Assert.AreEqual(1000, ticket.CashoutStake);
            Assert.AreEqual(2132, ticket.CashoutPercent);
            Assert.AreEqual(ticket.CashoutPercent, dto.CashoutPercent);
            Assert.IsNull(ticket.BetCashouts);
            Assert.IsNull(dto.BetCashout);
        }

        [TestMethod]
        public void BuildTicketBetCashoutTest()
        {
            var tb = TicketCashoutBuilder.Create();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .AddBetCashout("bet-id-01", 1000, 2132)
                           .AddBetCashout("bet-id-02", 1000, null)
                           .BuildTicket();
            var dto = new TicketCashoutMapper().Map(ticket);

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.IsNull(ticket.CashoutStake);
            Assert.IsNull(ticket.CashoutPercent);
            Assert.IsNotNull(ticket.BetCashouts);
            Assert.AreEqual(2, ticket.BetCashouts.Count());
            Assert.AreEqual(ticket.CashoutPercent, dto.CashoutPercent);
            Assert.AreEqual("bet-id-01", dto.BetCashout.First().Id);
            Assert.AreEqual("bet-id-02", dto.BetCashout.ToList()[1].Id);
            Assert.AreEqual(1000, dto.BetCashout.First().CashoutStake);
            Assert.AreEqual(1000, dto.BetCashout.ToList()[1].CashoutStake);
            Assert.AreEqual(2132, dto.BetCashout.First().CashoutPercent);
            Assert.IsNull(dto.BetCashout.ToList()[1].CashoutPercent);
        }

        [TestMethod]
        public void BuildTicketValidPercentTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.SetCashoutPercent(1)
              .SetCashoutPercent(1000000)
              .SetCashoutPercent(10101);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketBetCashoutMissingBetIdTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.AddBetCashout("", 1000, 1220);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketBetCashoutStakeLowTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.AddBetCashout("bet-id-01", 0, 1220);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketBetCashoutPercentLowTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.AddBetCashout("bet-id-01", 1000, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketBetCashoutPercentHighTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.AddBetCashout("bet-id-01", 1000, 1000001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketTooLowPercentTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.SetCashoutPercent(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketTooHighPercentTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.SetCashoutPercent(1000001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketBetCashoutAndStakePercentTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.SetTicketId("ticket-" + SR.I1000P)
              .SetBookmakerId(SR.I1000)
              .SetCashoutStake(1000)
              .SetCashoutPercent(2132)
              .AddBetCashout("bet-id-02", 123, null)
              .BuildTicket();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketBetCashoutAndPercentTest()
        {
            var tb = TicketCashoutBuilder.Create();
            tb.SetTicketId("ticket-" + SR.I1000P)
              .SetBookmakerId(SR.I1000)
              .SetCashoutPercent(2132)
              .AddBetCashout("bet-id-02", 123, null)
              .BuildTicket();
        }
    }
}