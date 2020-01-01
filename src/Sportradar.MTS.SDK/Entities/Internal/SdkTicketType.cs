/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Internal
{
    internal enum SdkTicketType
    {
        Ticket = 0,

        TicketCancel = 1,

        TicketAck = 2,

        TicketCancelAck = 3,

        TicketReofferCancel = 10,

        TicketCashout = 20,

        TicketNonSrSettle = 30,

        TicketResponse = 100,

        TicketCancelResponse = 101,

        TicketCashoutResponse = 102,

        TicketNonSrSettleResponse = 103
    }
}