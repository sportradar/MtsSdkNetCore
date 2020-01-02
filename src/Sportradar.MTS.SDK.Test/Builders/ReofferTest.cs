/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;
using Sportradar.MTS.SDK.Test.Helpers;
using static Sportradar.MTS.SDK.Test.Helpers.StaticRandom;

namespace Sportradar.MTS.SDK.Test.Builders
{
    [TestClass]
    public class ReofferTest
    {
        [TestMethod]
        public void BuildTicketReofferTest()
        {
            var ticket = TicketBuilderHelper.GetTicket();
            var ticketResponseDto = TicketBuilderHelper.GetTicketResponse(ticket, Status.Rejected, true, false);
            var ticketResponse = new TicketResponseMapper(null).Map(ticketResponseDto, S1000, null, ticketResponseDto.ToJson());
            Thread.Sleep(500);
            var reofferTicket = new BuilderFactoryHelper().BuilderFactory.CreateTicketReofferBuilder().Set(ticket, ticketResponse, "reofferTicket-" + I1000P).BuildTicket();

            Assert.IsNotNull(reofferTicket);

            TicketCompareHelper.Compare(ticket, reofferTicket, true, false);
        }

        [TestMethod]
        public void BuildTicketAltStakeTest()
        {
            var ticket = TicketBuilderHelper.GetTicket();
            var ticketResponseDto = TicketBuilderHelper.GetTicketResponse(ticket, Status.Rejected, false, true);
            var ticketResponse = new TicketResponseMapper(null).Map(ticketResponseDto, S1000, null, ticketResponseDto.ToJson());
            Thread.Sleep(500);
            var reofferTicket = new BuilderFactoryHelper().BuilderFactory.CreateAltStakeBuilder().Set(ticket, ticketResponse, "altStakeTicket-" + I1000P).BuildTicket();

            Assert.IsNotNull(reofferTicket);
            TicketCompareHelper.Compare(ticket, reofferTicket, false, true);
        }
    }
}