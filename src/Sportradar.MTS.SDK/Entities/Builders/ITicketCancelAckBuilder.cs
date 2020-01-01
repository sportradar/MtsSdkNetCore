/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes implementing builder for <see cref="ITicketCancelAckBuilder" />
    /// </summary>
    public interface ITicketCancelAckBuilder
    {
        /// <summary>
        /// Sets the ticket id
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketCancelAckBuilder"/></returns>
        ITicketCancelAckBuilder SetTicketId(string ticketId);

        /// <summary>
        /// Sets the ticket bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketCancelAckBuilder"/></returns>
        ITicketCancelAckBuilder SetBookmakerId(int bookmakerId);

        /// <summary>
        /// Sets the acknowledgment parameters
        /// </summary>
        /// <param name="markAccepted">If set to <c>true</c> [mark cancelled]</param>
        /// <param name="code">The code</param>
        /// <param name="message">The message</param>
        /// <returns>Returns a <see cref="ITicketCancelAckBuilder"/></returns>
        ITicketCancelAckBuilder SetAck(bool markAccepted, int code, string message);

        /// <summary>
        /// Builds the <see cref="ITicketCancelAck" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketCancelAck"/></returns>
        ITicketCancelAck BuildTicket();
    }
}
