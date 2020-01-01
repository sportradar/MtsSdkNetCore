/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Contract defining Ticket that can be send to the MTS
    /// </summary>
    public interface ITicketCancel : ISdkTicket
    {
        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        int BookmakerId { get; }

        /// <summary>
        /// Gets the cancellation code
        /// </summary>
        TicketCancellationReason Code { get; }

        /// <summary>
        /// Gets the cancel percent
        /// </summary>
        /// <value>The cancel percent</value>
        int? CancelPercent { get; }

        /// <summary>
        /// Gets the list of <see cref="IBetCancel"/>
        /// </summary>
        /// <value>The list of <see cref="IBetCancel"/></value>
        IEnumerable<IBetCancel> BetCancels { get; }
    }
}