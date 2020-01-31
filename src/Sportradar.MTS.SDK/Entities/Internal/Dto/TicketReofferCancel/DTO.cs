/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketReofferCancel
{
    internal partial class TicketReofferCancelDTO
    {
        public TicketReofferCancelDTO()
        { }

        public TicketReofferCancelDTO(long timestamp, string ticketId, int bookmakerId, string version)
        {
            _timestampUtc = timestamp;
            _ticketId = ticketId;
            _version = version;
            _sender = new Sender(bookmakerId);
        }

        public TicketReofferCancelDTO(ITicketReofferCancel ticket)
        {
            _timestampUtc = MtsTicketHelper.Convert(ticket.Timestamp);
            _ticketId = ticket.TicketId;
            _version = ticket.Version;
            _sender = new Sender(ticket.BookmakerId);
        }
    }
}