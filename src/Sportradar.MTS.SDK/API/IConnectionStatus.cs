using System;
using System.Collections.Generic;
using System.Text;
using Sportradar.MTS.SDK.Entities.EventArguments;

namespace Sportradar.MTS.SDK.API
{
    /// <summary>
    /// Defines a contract for classes providing information on connection to rabbit server
    /// </summary>
    public interface IConnectionStatus
    {
        /// <summary>
        /// Occurs when connection status change
        /// </summary>
        /// <remarks>On disconnection all ticket sending is disabled. Event ticket creation should be omitted to avoid having old ticket timestamp.</remarks>
        event EventHandler<ConnectionChangeEventArgs> ConnectionChange;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the time of when connection was made.
        /// </summary>
        /// <value>The connection time.</value>
        DateTime? ConnectionTime { get; }

        /// <summary>
        /// Gets the time of disconnection.
        /// </summary>
        /// <value>The disconnection time.</value>
        DateTime? DisconnectionTime { get; }

        /// <summary>
        /// Gets the last send ticket identifier.
        /// </summary>
        /// <value>The last send ticket identifier.</value>
        string LastSendTicketId { get; }

        /// <summary>
        /// Gets the last received ticket identifier.
        /// </summary>
        /// <value>The last received ticket identifier.</value>
        string LastReceivedTicketId { get; }
    }
}
