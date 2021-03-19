/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.Entities.EventArguments;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    internal class ConnectionStatus : IConnectionStatus
    {
        private const int QueueLimit = 10;
        /// <summary>
        /// Occurs when connection status change
        /// </summary>
        public event EventHandler<ConnectionChangeEventArgs> ConnectionChange;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected => ConnectionTime != null && DisconnectionTime == null;

        /// <summary>
        /// Gets the time of when last connection was made.
        /// </summary>
        /// <value>The connection time.</value>
        public DateTime? ConnectionTime { get; private set; }

        /// <summary>
        /// Gets the time of last disconnection.
        /// </summary>
        /// <value>The disconnection time.</value>
        public DateTime? DisconnectionTime { get; private set; }

        /// <summary>
        /// Gets the last send ticket identifier.
        /// </summary>
        /// <value>The last send ticket identifier.</value>
        public string LastSendTicketId { get; private set; }

        /// <summary>
        /// Gets the last received ticket identifier.
        /// </summary>
        /// <value>The last received ticket identifier.</value>
        public string LastReceivedTicketId { get; private set; }

        private readonly Queue<string> _lastSendTicketIds = new Queue<string>(QueueLimit);
        private readonly Queue<string> _lastReceivedTicketIds = new Queue<string>(QueueLimit);

        private readonly object _lock = new object();

        public ConnectionStatus()
        {
            ConnectionTime = null;
            DisconnectionTime = null;
            LastSendTicketId = null;
            LastReceivedTicketId = null;
        }

        internal void Connect(string message)
        {
            lock (_lock)
            {
                if (!IsConnected)
                {
                    ConnectionTime = DateTime.Now;
                    DisconnectionTime = null;
                    ConnectionChange?.Invoke(this, new ConnectionChangeEventArgs(IsConnected, message));
                }
            }
        }

        internal void Disconnect(string message)
        {
            lock (_lock)
            {
                if (IsConnected)
                {
                    ConnectionTime = null;
                    DisconnectionTime = DateTime.Now;
                    ConnectionChange?.Invoke(this, new ConnectionChangeEventArgs(IsConnected, message));
                }
            }
        }

        internal void TicketSend(string ticketId)
        {
            lock (_lock)
            {
                LastSendTicketId = ticketId;
                _lastSendTicketIds.Enqueue(ticketId);
                if (_lastSendTicketIds.Count > QueueLimit)
                {
                    _lastSendTicketIds.TryDequeue(out _);
                }
            }
        }

        internal void TicketReceived(string ticketId)
        {
            lock (_lock)
            {
                LastReceivedTicketId = ticketId;
                _lastReceivedTicketIds.Enqueue(ticketId);
                if (_lastReceivedTicketIds.Count > QueueLimit)
                {
                    _lastReceivedTicketIds.TryDequeue(out _);
                }
            }
        }
    }
}
