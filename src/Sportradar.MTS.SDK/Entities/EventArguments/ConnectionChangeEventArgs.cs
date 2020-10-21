/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;

namespace Sportradar.MTS.SDK.Entities.EventArguments
{
    /// <summary>
    /// An event argument used by events raised to provide message about connection state to the rabbit server
    /// </summary>
    public class ConnectionChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets an indicator if the connection is on or not
        /// </summary>
        public bool IsConnected { get; }

        /// <summary>
        /// Gets the message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionChangeEventArgs"/> class
        /// </summary>
        /// <param name="isConnected">The ticketId</param>
        /// <param name="msg">The message</param>
        public ConnectionChangeEventArgs(bool isConnected, string msg)
        {
            IsConnected = isConnected;
            Message = msg;
        }
    }
}