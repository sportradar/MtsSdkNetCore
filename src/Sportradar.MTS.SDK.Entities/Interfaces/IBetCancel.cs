/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for bet-level cancellation
    /// </summary>
    public interface IBetCancel
    {
        /// <summary>
        /// Gets the id of the bet
        /// </summary>
        string BetId { get; }

        /// <summary>
        /// Gets the cancel percent of the assigned bet
        /// </summary>
        /// <value>The cancel percent</value>
        int? CancelPercent { get; }
    }
}
