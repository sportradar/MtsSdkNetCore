/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    internal class TicketCancelAckMapper : ITicketMapper<ITicketCancelAck, TicketCancelAckDTO>
    {
        public TicketCancelAckDTO Map(ITicketCancelAck source)
        {
            return new TicketCancelAckDTO(source);
        }
    }
}