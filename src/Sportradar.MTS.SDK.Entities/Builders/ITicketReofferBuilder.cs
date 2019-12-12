/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes implementing builder for <see cref="ITicket" /> reoffer
    /// </summary>
    public interface ITicketReofferBuilder : ISdkTicketBuilder
    {
        /// <summary>
        /// Sets the original ticket and the ticket response
        /// </summary>
        /// <param name="ticket">The original ticket</param>
        /// <param name="ticketResponse">The ticket response from which the stake info will be used</param>
        /// <param name="newTicketId">The new reoffer ticket id</param>
        /// <returns>Returns the <see cref="ITicketReofferBuilder"/></returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        ITicketReofferBuilder Set(ITicket ticket, ITicketResponse ticketResponse, string newTicketId = null);

        /// <summary>
        /// Sets the original ticket and the ticket response
        /// </summary>
        /// <param name="ticket">The original ticket</param>
        /// <param name="newStake">The new stake value which will be used to set bet stake</param>
        /// <param name="newTicketId">The new reoffer ticket id</param>
        /// <returns>Returns the <see cref="ITicketReofferBuilder"/></returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        ITicketReofferBuilder Set(ITicket ticket, long newStake, string newTicketId = null);

        /// <summary>
        /// Builds the new <see cref="ITicket" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicket"/></returns>
        ITicket BuildTicket();
    }
}
