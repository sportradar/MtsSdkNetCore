/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes implementing builder for <see cref="ITicketAckBuilder" />
    /// </summary>
    public interface ITicketAckBuilder
    {
        /// <summary>
        /// Sets the ticket id
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketAckBuilder"/></returns>
        ITicketAckBuilder SetTicketId(string ticketId);

        /// <summary>
        /// Sets the ticket bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketAckBuilder"/></returns>
        ITicketAckBuilder SetBookmakerId(int bookmakerId);

        /// <summary>
        /// Sets the acknowledgment parameters
        /// </summary>
        /// <param name="markAccepted">If set to <c>true</c> [mark accepted]</param>
        /// <param name="code">The code</param>
        /// <param name="message">The message</param>
        /// <returns>Returns a <see cref="ITicketAckBuilder"/></returns>
        ITicketAckBuilder SetAck(bool markAccepted, int code, string message);

        /// <summary>
        /// Builds the <see cref="ITicketAck" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketAck"/></returns>
        ITicketAck BuildTicket();
    }
}
