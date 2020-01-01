/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancel;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    internal class TicketCancelMapper : ITicketMapper<ITicketCancel, TicketCancelDTO>
    {
        public TicketCancelDTO Map(ITicketCancel source)
        {
            return new TicketCancelDTO(source);
        }
    }
}