/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="IAlternativeStake"/>
    /// </summary>
    /// <seealso cref="IAlternativeStake" />
    public class AlternativeStake : IAlternativeStake
    {
        /// <summary>
        /// Gets the stake
        /// </summary>
        /// <value>The stake</value>
        public long Stake { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativeStake"/> class
        /// </summary>
        /// <param name="stake">The stake</param>
        public AlternativeStake(long stake)
        {
            Guard.Argument(stake, nameof(stake)).InRange(1, 1000000000000000000 - 1);

            Stake = stake;
        }
    }
}