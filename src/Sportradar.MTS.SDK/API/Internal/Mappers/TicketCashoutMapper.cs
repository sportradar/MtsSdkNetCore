/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashout;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    internal class TicketCashoutMapper : ITicketMapper<ITicketCashout, TicketCashoutDTO>
    {
        public TicketCashoutDTO Map(ITicketCashout source)
        {
            return new TicketCashoutDTO(source);
        }
    }
}