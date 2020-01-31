/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    internal partial class TicketDTO
    {
        public TicketDTO()
        { }

        public TicketDTO(ITicket ticket)
        {
            _ticket = new Ticket(ticket);
        }
    }
}