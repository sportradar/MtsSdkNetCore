/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes building a <see cref="ITicketNonSrSettleBuilder" />
    /// Implements the <see cref="ISdkTicketBuilder" />
    /// </summary>
    /// <seealso cref="ISdkTicketBuilder" />
    public interface ITicketNonSrSettleBuilder : ISdkTicketBuilder
    {
        /// <summary>
        /// Sets the ticket id to non-sportradar settle
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketNonSrSettleBuilder"/></returns>
        /// <value>Unique ticket id (in the client's system)</value>
        ITicketNonSrSettleBuilder SetTicketId(string ticketId);

        /// <summary>
        /// Sets the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketNonSrSettleBuilder"/></returns>
        ITicketNonSrSettleBuilder SetBookmakerId(int bookmakerId);

        /// <summary>
        /// Sets the non-sportradar settle stake
        /// </summary>
        /// <param name="stake">The non-sportradar settle stake</param>
        /// <returns>Returns a <see cref="ITicketNonSrSettleBuilder"/></returns>
        ITicketNonSrSettleBuilder SetNonSrSettleStake(long stake);

        /// <summary>
        /// Build a <see cref="ITicketNonSrSettle" />
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <param name="stake">The non-sportradar settle stake</param>
        /// <returns>ITicketNonSrSettle</returns>
        ITicketNonSrSettle BuildTicket(string ticketId, int bookmakerId, long stake);

        /// <summary>
        /// Builds the <see cref="ITicketNonSrSettle" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketNonSrSettle"/></returns>
        ITicketNonSrSettle BuildTicket();
    }
}

