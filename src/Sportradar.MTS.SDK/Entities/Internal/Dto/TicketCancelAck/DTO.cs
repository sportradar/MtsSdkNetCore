/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck
{
    public partial class TicketCancelAckDTO
    {
        public TicketCancelAckDTO()
        { }

        public TicketCancelAckDTO(string ticketId, TicketCancelAckDTOTicketCancelStatus ticketCancelStatus, int code, string message, long timestamp, string version, int ticketBookmakerId)
        {
            _ticketId = ticketId;
            _ticketCancelStatus = ticketCancelStatus;
            _code = code;
            _message = message;
            _timestampUtc = timestamp;
            _version = version;
            _sender = new Sender(ticketBookmakerId);
        }

        public TicketCancelAckDTO(ITicketCancelAck ticket)
        {
            _ticketId = ticket.TicketId;
            _ticketCancelStatus = MtsTicketHelper.Convert(ticket.TicketCancelStatus);
            _code = ticket.Code;
            _message = ticket.Message;
            _timestampUtc = MtsTicketHelper.Convert(ticket.Timestamp);
            _version = ticket.Version;
            _sender = new Sender(ticket.BookmakerId);
        }
    }
}