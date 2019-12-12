/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for alternative stake, mutually exclusive with reoffer
    /// </summary>
    public interface IAlternativeStake
    {
        /// <summary>
        /// Gets the stake
        /// </summary>
        long Stake { get; }
    }
}