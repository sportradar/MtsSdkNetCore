/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancel
{
    public partial class TicketCancelDTO
    {
        public TicketCancelDTO()
        { }

        public TicketCancelDTO(ITicketCancel ticket)
        {
            _cancel = new Cancel(ticket);
        }
    }
}