/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for object carrying information about rejection
    /// </summary>
    public interface IRejectionInfo
    {
        /// <summary>
        /// Gets the rejected selection's related selection id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the rejected selection's related Betradar event (match or outright) id
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets the rejected selection's related Odds
        /// </summary>
        int? Odds { get; }
    }
}