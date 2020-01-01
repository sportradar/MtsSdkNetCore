/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.EventArguments
{
    /// <summary>
    /// An event argument used by events raised to provide message about sdk or ticket state
    /// </summary>
    public class TicketMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a ticketId
        /// </summary>
        public string TicketId { get; }

        /// <summary>
        /// Gets a associated ticket
        /// </summary>
        public ISdkTicket Ticket { get; }

        /// <summary>
        /// Gets the message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketMessageEventArgs"/> class
        /// </summary>
        /// <param name="id">The ticketId</param>
        /// <param name="ticket">The associated ticket</param>
        /// <param name="msg">The message</param>
        public TicketMessageEventArgs(string id, ISdkTicket ticket, string msg)
        {
            TicketId = id;
            Ticket = ticket;
            Message = msg;
        }
    }
}