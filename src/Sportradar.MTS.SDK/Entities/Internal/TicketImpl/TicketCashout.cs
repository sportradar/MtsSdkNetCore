/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="ITicketCashout"/>
    /// </summary>
    /// <seealso cref="ITicketCashout" />
    [Serializable]
    internal class TicketCashout : ITicketCashout
    {
        /// <summary>
        /// Gets the timestamp of ticket placement (UTC)
        /// </summary>
        /// <value>The timestamp</value>
        public DateTime Timestamp { get; }
        /// <summary>
        /// Gets the ticket id
        /// </summary>
        /// <value>Unique ticket id (in the client's system)</value>
        public string TicketId { get; }
        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <value>The bookmaker identifier</value>
        public int BookmakerId { get; }
        /// <summary>
        /// Gets the cashout stake
        /// </summary>
        /// <value>The cashout stake</value>
        public long? CashoutStake { get; }
        /// <summary>
        /// Gets the cashout percent
        /// </summary>
        /// <value>The cashout percent</value>
        public int? CashoutPercent { get; }
        /// <summary>
        /// Gets the list of <see cref="IBetCashout"/>
        /// </summary>
        /// <value>The list of <see cref="IBetCashout"/></value>
        public IEnumerable<IBetCashout> BetCashouts { get; }
        /// <summary>
        /// Gets the ticket format version
        /// </summary>
        /// <value>The version</value>
        public string Version { get; }
        /// <summary>
        /// Gets the correlation identifier
        /// </summary>
        /// <value>The correlation identifier</value>
        /// <remarks>Only used to relate ticket with its response</remarks>
        public string CorrelationId { get; }

        public string ToJson()
        {
            var dto = EntitiesMapper.Map(this);
            return dto.ToJson();
        }

        [JsonConstructor]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        private TicketCashout(DateTime timestamp, string ticketId, int bookmakerId, long? cashoutStake, int? cashoutPercent, IEnumerable<IBetCashout> betCashouts, string version, string correlationId)
        {
            ValidateConstructorParameters(ticketId, bookmakerId, cashoutStake, cashoutPercent, betCashouts);

            Timestamp = timestamp;
            TicketId = ticketId;
            BookmakerId = bookmakerId;
            CashoutStake = cashoutStake;
            CashoutPercent = cashoutPercent;
            BetCashouts = betCashouts;
            Version = version;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCashout"/> class
        /// </summary>
        /// <param name="ticketId">The ticket identifier</param>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <param name="stake">The cashout stake</param>
        /// <param name="percent">The cashout percent</param>
        /// <param name="betCashouts">The list of <see cref="IBetCashout"/></param>
        public TicketCashout(string ticketId, int bookmakerId, long? stake, int? percent, IReadOnlyCollection<IBetCashout> betCashouts)
        {
            ValidateConstructorParameters(ticketId, bookmakerId, stake, percent, betCashouts);

            TicketId = ticketId;
            BookmakerId = bookmakerId;
            CashoutStake = stake;
            CashoutPercent = percent;
            BetCashouts = betCashouts;
            Timestamp = DateTime.UtcNow;
            Version = TicketHelper.MtsTicketVersion;
            CorrelationId = TicketHelper.GenerateTicketCorrelationId();
        }

        private void ValidateConstructorParameters(string ticketId, int bookmakerId, long? stake, int? percent, IEnumerable<IBetCashout> betCashouts)
        {
            Guard.Argument(ticketId).Require(TicketHelper.ValidateTicketId(ticketId));
            Guard.Argument(bookmakerId, nameof(bookmakerId)).Positive();
            Guard.Argument(stake, nameof(stake)).Require(stake >= 0 || percent >= 0 || (betCashouts != null && betCashouts.Any()));

            if (percent != null && stake == null)
            {
                throw new ArgumentException("If percent is set, also stake must be.");
            }

            if (betCashouts != null && stake != null)
            {
                throw new ArgumentException("Stake and/or Percent cannot be set at the same time as BetCashouts.");
            }
        }
    }
}