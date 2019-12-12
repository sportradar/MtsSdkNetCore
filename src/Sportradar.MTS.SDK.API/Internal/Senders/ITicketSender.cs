/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Entities.EventArguments;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.API.Internal.Senders
{
    /// <summary>
    /// Interface ITicketSender
    /// </summary>
    /// <seealso cref="IOpenable" />
    public interface ITicketSender : IOpenable
    {
        /// <summary>
        /// Raised when the attempt to send ticket failed
        /// </summary>
        event EventHandler<TicketSendFailedEventArgs> TicketSendFailed;

        /// <summary>
        /// Sends the ticket
        /// </summary>
        /// <param name="ticket">The ticket</param>
        void SendTicket(ISdkTicket ticket);

        /// <summary>
        /// Gets the sent ticket
        /// </summary>
        /// <param name="ticketId">The ticket identifier</param>
        /// <returns>ISdkTicket</returns>
        ISdkTicket GetSentTicket(string ticketId);

        /// <summary>
        /// Gets the get cache timeout
        /// </summary>
        /// <value>The get cache timeout</value>
        int GetCacheTimeout(ISdkTicket ticket);
    }
}
 