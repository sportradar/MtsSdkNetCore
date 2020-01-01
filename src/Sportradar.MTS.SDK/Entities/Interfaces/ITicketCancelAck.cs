/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Object that is send to MTS to acknowledge ticket cancellation
    /// </summary>
    public interface ITicketCancelAck : ISdkTicket
    {
        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        int BookmakerId { get; }

        /// <summary>
        /// Gets the code
        /// </summary>
        int Code { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the status of the ticket cancel
        /// </summary>
        TicketCancelAckStatus TicketCancelStatus { get; }
    }
}