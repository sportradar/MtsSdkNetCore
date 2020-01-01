/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.ComponentModel.DataAnnotations;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for classes representing base sdk ticket
    /// </summary>
    public interface ISdkTicket
    {
        /// <summary>
        /// Gets the ticket id
        /// </summary>
        /// <value>Unique ticket id (in the client's system)</value>
        [Required(AllowEmptyStrings = false)]
        string TicketId { get; }

        /// <summary>
        /// Gets the timestamp of ticket placement (UTC)
        /// </summary>
        [Required]
        DateTime Timestamp { get; }

        /// <summary>
        /// Gets the ticket format version
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        string Version { get; }

        /// <summary>
        /// Gets the correlation identifier
        /// </summary>
        /// <remarks>Only used to relate ticket with its response</remarks>
        /// <value>The correlation identifier</value>
        [Required(AllowEmptyStrings = false)]
        string CorrelationId { get; }

        /// <summary>
        /// Gets the json representation of the ticket which is sent to or received from MTS
        /// </summary>
        /// <returns>The json representation of the ticket which is sent to or received from MTS</returns>
        string ToJson();
    }
}