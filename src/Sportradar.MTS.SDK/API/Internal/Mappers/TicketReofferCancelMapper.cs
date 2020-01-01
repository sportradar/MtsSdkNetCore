/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketReofferCancel;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    internal class TicketReofferCancelMapper : ITicketMapper<ITicketReofferCancel, TicketReofferCancelDTO>
    {
        public TicketReofferCancelDTO Map(ITicketReofferCancel source)
        {
            return new TicketReofferCancelDTO(source);
        }
    }
}