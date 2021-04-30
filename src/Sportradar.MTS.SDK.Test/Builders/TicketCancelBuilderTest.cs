/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Test.Helpers;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;

namespace Sportradar.MTS.SDK.Test.Builders
{
    [TestClass]
    public class TicketCancelBuilderTest
    {
        [TestMethod]
        public void BuildBaseTicketTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .SetCode(TicketCancellationReason.BookmakerBackofficeTriggered)
                           .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.AreEqual(TicketHelper.MtsTicketVersion, ticket.Version);
            Assert.IsTrue(!string.IsNullOrEmpty(ticket.TicketId));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoCodeTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoBookmakerIdTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetCode(TicketCancellationReason.BookmakerBackofficeTriggered)
                           .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketWithNoTicketIdTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            var ticket = tb.SetBookmakerId(SR.I1000)
                           .SetCode(TicketCancellationReason.BookmakerBackofficeTriggered)
                           .BuildTicket();

            Assert.IsNull(ticket);
        }

        [TestMethod]
        public void BuildTicketPercentTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .SetCode(TicketCancellationReason.BookmakerBackofficeTriggered)
                           .SetCancelPercent(2132)
                           .BuildTicket();
            var dto = new TicketCancelMapper().Map(ticket);

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.IsNotNull(ticket.CancelPercent);
            Assert.AreEqual(2132, ticket.CancelPercent);
            Assert.AreEqual(ticket.CancelPercent, dto.Cancel.CancelPercent);
            Assert.IsNull(ticket.BetCancels);
            Assert.IsNull(dto.Cancel.BetCancel);
        }

        [TestMethod]
        public void BuildTicketBetCancelTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            var ticket = tb.SetTicketId("ticket-" + SR.I1000P)
                           .SetBookmakerId(SR.I1000)
                           .SetCode(TicketCancellationReason.BookmakerBackofficeTriggered)
                           .AddBetCancel("bet-id-01", 2132)
                           .AddBetCancel("bet-id-02", null)
                           .BuildTicket();
            var dto = new TicketCancelMapper().Map(ticket);

            Assert.IsNotNull(ticket);
            Assert.IsTrue(ticket.Timestamp > DateTime.Today.ToUniversalTime());
            Assert.IsNull(ticket.CancelPercent);
            Assert.AreEqual(2, ticket.BetCancels.Count());
            Assert.AreEqual(ticket.CancelPercent, dto.Cancel.CancelPercent);
            Assert.IsNotNull(ticket.BetCancels);
            Assert.IsNotNull(dto.Cancel.BetCancel);
            Assert.AreEqual("bet-id-01", dto.Cancel.BetCancel.First().Id);
            Assert.AreEqual("bet-id-02", dto.Cancel.BetCancel.ToList()[1].Id);
            Assert.AreEqual(2132, dto.Cancel.BetCancel.First().CancelPercent);
            Assert.IsNull(dto.Cancel.BetCancel.ToList()[1].CancelPercent);
        }

        [TestMethod]
        public void BuildTicketValidPercentTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            tb.SetCancelPercent(1)
              .SetCancelPercent(1000000)
              .SetCancelPercent(10101);

            Assert.IsNotNull(tb);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildTicketBetCancelMissingBetIdTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            tb.AddBetCancel("", 1220);

            Assert.IsNotNull(tb);
        }

        [TestMethod]
        public void BuildTicketTooLowPercentTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            tb.SetCancelPercent(0);

            Assert.IsNotNull(tb);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketTooHighPercentTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            tb.SetCancelPercent(1000001);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
        public void BuildTicketBetCancelAndPercentTest()
        {
            var tb = new BuilderFactoryHelper().BuilderFactory.CreateTicketCancelBuilder();
            tb.SetTicketId("ticket-" + SR.I1000P)
              .SetBookmakerId(SR.I1000)
              .SetCode(TicketCancellationReason.BookmakerBackofficeTriggered)
              .SetCancelPercent(2132)
              .AddBetCancel("bet-id-02", null)
              .BuildTicket();
        }
    }
}