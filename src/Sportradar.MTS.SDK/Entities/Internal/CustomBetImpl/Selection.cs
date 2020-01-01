/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Sportradar.MTS.SDK.Entities.Interfaces.CustomBet;

namespace Sportradar.MTS.SDK.Entities.Internal.CustomBetImpl
{
    /// <summary>
    /// Implements methods used to provides an requested selection
    /// </summary>
    internal class Selection : ISelection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selection"/> class
        /// </summary>
        /// <param name="eventId">a <see cref="string"/> representing the event id</param>
        /// <param name="marketId">a value representing the market id</param>
        /// <param name="specifiers">a value representing the specifiers</param>
        /// <param name="outcomeId">a value representing the outcome id</param>
        internal Selection(string eventId, int marketId, string specifiers, string outcomeId)
        {
            if (eventId == null)
            {
                throw new ArgumentNullException(nameof(eventId));
            }

            if (specifiers == null)
            {
                throw new ArgumentNullException(nameof(specifiers));
            }

            if (outcomeId == null)
            {
                throw new ArgumentNullException(nameof(outcomeId));
            }

            EventId = eventId;
            MarketId = marketId;
            Specifiers = specifiers;
            OutcomeId = outcomeId;
        }

        public string EventId { get; }
        public int MarketId { get; }
        public string Specifiers { get; }
        public string OutcomeId { get; }
    }
}
