/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettle
{
    internal partial class TicketNonSrSettleDTO
    {
        public TicketNonSrSettleDTO()
        { }

        public TicketNonSrSettleDTO(ITicketNonSrSettle ticket)
        {
            _timestampUtc = MtsTicketHelper.Convert(ticket.Timestamp);
            _ticketId = ticket.TicketId;
            _nonSrSettleStake = ticket.NonSrSettleStake;
            _version = ticket.Version;
            _sender = new Sender(ticket.BookmakerId);
        }
    }
}