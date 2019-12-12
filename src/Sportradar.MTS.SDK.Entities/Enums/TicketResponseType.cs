/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Defines possible values for responses from MTS
    /// </summary>
    public enum TicketResponseType
    {
        /// <summary>
        /// The ticket
        /// </summary>
        Ticket = 1,

        /// <summary>
        /// The ticket cancel
        /// </summary>
        TicketCancel = 2,

        /// <summary>
        /// The ticket cashout
        /// </summary>
        TicketCashout = 3,

        /// <summary>
        /// The ticket non-sportradar settle
        /// </summary>
        TicketNonSrSettle = 4
    }
}