/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces.CustomBet
{
    /// <summary>
    /// Provides an requested selection
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Gets the event id.
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets the market id.
        /// </summary>
        int MarketId { get; }

        /// <summary>
        /// Gets the specifiers.
        /// </summary>
        string Specifiers { get; }

        /// <summary>
        /// Gets the outcome id.
        /// </summary>
        string OutcomeId { get; }
    }
}
