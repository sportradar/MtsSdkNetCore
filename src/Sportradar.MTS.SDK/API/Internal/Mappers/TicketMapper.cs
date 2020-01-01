/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    internal class TicketMapper : ITicketMapper<ITicket, TicketDTO>
    {
        public TicketDTO Map(ITicket source)
        {
            return new TicketDTO(source);
        }
    }
}