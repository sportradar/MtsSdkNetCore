/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck
{
    internal partial class TicketAckDTO
    {
        public TicketAckDTO()
        {
        }

        public TicketAckDTO(string ticketId, int bookmakerId, TicketAckStatus ackStatus, string message, int code, DateTime timestamp, string version)
        {
            TicketId = ticketId;
            Sender = new Sender(bookmakerId);
            Code = code;
            Message = message;
            TicketStatus = MtsTicketHelper.Convert(ackStatus);
            TimestampUtc = MtsTicketHelper.Convert(timestamp);
            Version = version;
        }

        public TicketAckDTO(ITicketAck ticket)
        {
            TicketId = ticket.TicketId;
            Sender = new Sender(ticket.BookmakerId);
            Code = ticket.Code;
            Message = ticket.Message;
            TicketStatus = MtsTicketHelper.Convert(ticket.TicketStatus);
            TimestampUtc = MtsTicketHelper.Convert(ticket.Timestamp);
            Version = ticket.Version;
        }
    }
}