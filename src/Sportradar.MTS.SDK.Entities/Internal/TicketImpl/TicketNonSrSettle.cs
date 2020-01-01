/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="ITicketNonSrSettle"/>
    /// </summary>
    /// <seealso cref="ITicketNonSrSettle" />
    [Serializable]
    class TicketNonSrSettle : ITicketNonSrSettle
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
        /// Gets the non-sportradar settle stake
        /// </summary>
        /// <value>The non-sportradar settle stake</value>
        public long? NonSrSettleStake { get; }
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
        private TicketNonSrSettle(DateTime timestamp, string ticketId, int bookmakerId, long? nonSrSettleStake, string version, string correlationId)
        {
            Timestamp = timestamp;
            TicketId = ticketId;
            BookmakerId = bookmakerId;
            NonSrSettleStake = nonSrSettleStake;
            Version = version;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ITicketNonSrSettle"/> class
        /// </summary>
        /// <param name="ticketId">The ticket identifier</param>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <param name="stake">The non-sportradar settle stake</param>
        public TicketNonSrSettle(string ticketId, int bookmakerId, long stake)
        {
            Guard.Argument(ticketId, nameof(ticketId)).Require(TicketHelper.ValidateTicketId(ticketId));
            Guard.Argument(bookmakerId, nameof(bookmakerId)).Positive();
            Guard.Argument(stake, nameof(stake)).NotNegative();

            TicketId = ticketId;
            BookmakerId = bookmakerId;
            NonSrSettleStake = stake;
            Timestamp = DateTime.UtcNow;
            Version = TicketHelper.MtsTicketVersion;
            CorrelationId = TicketHelper.GenerateTicketCorrelationId();
        }
    }
}
