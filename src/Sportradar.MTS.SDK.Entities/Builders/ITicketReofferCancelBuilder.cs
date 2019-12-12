/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes building a <see cref="ITicketReofferCancel" />
    /// </summary>
    public interface ITicketReofferCancelBuilder : ISdkTicketBuilder
    {
        /// <summary>
        /// Sets the reoffer ticket id to cancel
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketReofferCancelBuilder"/></returns>
        /// <value>Unique reoffer ticket id (in the client's system)</value>
        ITicketReofferCancelBuilder SetTicketId(string ticketId);

        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketReofferCancelBuilder"/></returns>
        ITicketReofferCancelBuilder SetBookmakerId(int bookmakerId);

        /// <summary>
        /// Build a <see cref="ITicketReofferCancel" />
        /// </summary>
        /// <param name="ticketId">The reoffer ticket id</param>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns an instance of <see cref="ITicketReofferCancel"/></returns>
        ITicketReofferCancel BuildTicket(string ticketId, int bookmakerId);

        /// <summary>
        /// Builds the <see cref="ITicketReofferCancel" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketReofferCancel"/></returns>
        ITicketReofferCancel BuildTicket();
    }
}
