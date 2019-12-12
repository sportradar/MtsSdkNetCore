/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for bet-level response details
    /// </summary>
    public interface IBetDetail
    {
        /// <summary>
        /// Gets the id of the bet
        /// </summary>
        string BetId { get; }

        /// <summary>
        /// Gets the bet response reason.
        /// </summary>
        IResponseReason Reason { get; }

        /// <summary>
        /// Gets the array of selection details
        /// </summary>
        IEnumerable<ISelectionDetail> SelectionDetails { get; }

        /// <summary>
        /// Gets the bet reoffer details (mutually exclusive with <see cref="IAlternativeStake"/>)
        /// </summary>
        IBetReoffer Reoffer { get; }

        /// <summary>
        /// Gets the alternative stake, mutually exclusive with <see cref="IBetReoffer"/>
        /// </summary>
        IAlternativeStake AlternativeStake { get; }
    }
}