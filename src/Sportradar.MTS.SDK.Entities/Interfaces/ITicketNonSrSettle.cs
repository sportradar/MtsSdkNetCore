/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Contract defining Ticket that can be send to the MTS
    /// </summary>
    public interface ITicketNonSrSettle : ISdkTicket
    {
        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        int BookmakerId { get; }
        /// <summary>
        /// Gets the non-sportradar settle stake
        /// </summary>
        /// <remarks>If value is 0 means the ticket was not settled</remarks>
        long? NonSrSettleStake { get; }
    }
}