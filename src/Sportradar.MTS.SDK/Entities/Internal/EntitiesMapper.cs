/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancel;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashout;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketReofferCancel;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettle;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    internal static class EntitiesMapper
    {
        public static TicketDTO Map(ITicket source)
        {
            return new TicketDTO(source);
        }

        public static TicketAckDTO Map(ITicketAck source)
        {
            return new TicketAckDTO(source);
        }

        public static TicketCancelDTO Map(ITicketCancel source)
        {
            return new TicketCancelDTO(source);
        }

        public static TicketCancelAckDTO Map(ITicketCancelAck source)
        {
            return new TicketCancelAckDTO(source);
        }

        public static TicketCashoutDTO Map(ITicketCashout source)
        {
            return new TicketCashoutDTO(source);
        }

        public static TicketReofferCancelDTO Map(ITicketReofferCancel source)
        {
            return new TicketReofferCancelDTO(source);
        }

        internal static TicketNonSrSettleDTO Map(TicketNonSrSettle source)
        {
            return new TicketNonSrSettleDTO(source);
        }
    }
}