/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;

namespace Sportradar.MTS.SDK.Entities.EventArguments
{
    /// <summary>
    /// An event argument used by events raised when an attempt to send ticket failed
    /// </summary>
    public class TicketSendFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a ticketId
        /// </summary>
        public string TicketId { get; }

        /// <summary>
        /// Gets a ticket representation as json string
        /// </summary>
        public string TicketBody { get; }

        /// <summary>
        /// Gets the error message
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketSendFailedEventArgs"/> class
        /// </summary>
        /// <param name="id">The ticketId</param>
        /// <param name="body">The json representation of the ticket</param>
        /// <param name="msg">The reason why sending failed</param>
        public TicketSendFailedEventArgs(string id, string body, string msg)
        {
            TicketId = id;
            TicketBody = body;
            ErrorMessage = msg;
        }
    }
}