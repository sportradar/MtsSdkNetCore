/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Linq;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashout
{
    public partial class TicketCashoutDTO
    {
        public TicketCashoutDTO()
        { }

        public TicketCashoutDTO(ITicketCashout ticket)
        {
            _timestampUtc = MtsTicketHelper.Convert(ticket.Timestamp);
            _ticketId = ticket.TicketId;
            _cashoutStake = ticket.CashoutStake;
            _version = ticket.Version;
            _sender = new Sender(ticket.BookmakerId);
            _cashoutPercent = ticket.CashoutPercent;
            _betCashout = ticket.BetCashouts?.ToList().ConvertAll(c => new Anonymous(c));
        }
    }
}