/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for bet reoffer details, mutually exclusive with <see cref="IAlternativeStake"/>
    /// </summary>
    public interface IBetReoffer
    {
        /// <summary>
        /// Gets the stake
        /// </summary>
        long Stake { get; }

        /// <summary>
        /// Gets the reoffer type. If auto then stake will be present. If manual you should wait for reoffer stake over Reply channel.
        /// </summary>
        BetReofferType Type { get; }
    }
}