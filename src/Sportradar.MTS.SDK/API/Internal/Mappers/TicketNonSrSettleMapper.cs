/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettle;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    internal class TicketNonSrSettleMapper : ITicketMapper<ITicketNonSrSettle, TicketNonSrSettleDTO>
    {
        public TicketNonSrSettleDTO Map(ITicketNonSrSettle source)
        {
            return new TicketNonSrSettleDTO(source);
        }
    }
}