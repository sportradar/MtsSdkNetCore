/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    internal class TicketAckMapper : ITicketMapper<ITicketAck, TicketAckDTO>
    {
        public TicketAckDTO Map(ITicketAck source)
        {
            return new TicketAckDTO(source);
        }
    }
}