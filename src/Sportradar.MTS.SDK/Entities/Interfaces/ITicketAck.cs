/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for ticket acknowledgment ticket
    /// </summary>
    public interface ITicketAck : ISdkTicket
    {
        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        int BookmakerId { get; }

        /// <summary>
        /// Get the code
        /// </summary>
        int Code { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the status of the ticket
        /// </summary>
        TicketAckStatus TicketStatus { get; }
    }
}