/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Contract defining Ticket that can be send to the MTS
    /// </summary>
    public interface ITicketCashout : ISdkTicket
    {
        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        int BookmakerId { get; }

        /// <summary>
        /// Gets the cashout stake
        /// </summary>
        long? CashoutStake { get; }

        /// <summary>
        /// Gets the cashout percent
        /// </summary>
        /// <value>The cashout percent</value>
        int? CashoutPercent { get; }

        /// <summary>
        /// Gets the list of <see cref="IBetCashout"/>
        /// </summary>
        /// <value>The list of <see cref="IBetCashout"/></value>
        IEnumerable<IBetCashout> BetCashouts { get; }
    }
}