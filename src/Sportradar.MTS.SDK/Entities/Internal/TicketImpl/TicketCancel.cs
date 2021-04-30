/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="ITicketCancel"/>
    /// </summary>
    /// <seealso cref="ITicketCancel" />
    [Serializable]
    internal class TicketCancel : ITicketCancel
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
        /// Gets the cancellation code
        /// </summary>
        /// <value>The code</value>
        public TicketCancellationReason Code { get; }
        /// <summary>
        /// Gets the cancel percent
        /// </summary>
        /// <value>The cancel percent</value>
        public int? CancelPercent { get; }
        /// <summary>
        /// Gets the list of <see cref="IBetCancel"/>
        /// </summary>
        /// <value>The list of <see cref="IBetCancel"/></value>
        public IEnumerable<IBetCancel> BetCancels { get; }
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
            return dto.Cancel.ToJson();
        }

        [JsonConstructor]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        private TicketCancel(DateTime timestamp, 
                             string ticketId, 
                             int bookmakerId, 
                             TicketCancellationReason code, 
                             int? cancelPercent, 
                             IEnumerable<IBetCancel> betCancels, 
                             string version, 
                             string correlationId)
        {
            Timestamp = timestamp;
            TicketId = ticketId;
            BookmakerId = bookmakerId;
            Code = code;
            CancelPercent = cancelPercent;
            BetCancels = betCancels;
            Version = version;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCancel"/> class
        /// </summary>
        /// <param name="ticketId">The ticket identifier</param>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <param name="code">The code</param>
        /// <param name="percent">The percent of ticket to cancel</param>
        /// <param name="betCancels">The list of <see cref="IBetCancel"/></param>
        public TicketCancel(string ticketId, int bookmakerId, TicketCancellationReason code, int? percent, IReadOnlyCollection<IBetCancel> betCancels)
        {
            Guard.Argument(ticketId, nameof(ticketId)).Require(TicketHelper.ValidateTicketId(ticketId));
            Guard.Argument(bookmakerId, nameof(bookmakerId)).Positive();
            Guard.Argument(percent, nameof(percent)).Require(TicketHelper.ValidatePercent(percent));
            Guard.Argument(betCancels, nameof(betCancels)).Require(betCancels == null || betCancels.Any());

            if (percent != null && betCancels != null)
            {
                throw new ArgumentException("Percent and BetCancels cannot be set at the same time.");
            }

            TicketId = ticketId;
            BookmakerId = bookmakerId;
            Code = code;
            Timestamp = DateTime.UtcNow;
            Version = TicketHelper.MtsTicketVersion;
            CorrelationId = TicketHelper.GenerateTicketCorrelationId();
            CancelPercent = percent;
            BetCancels = betCancels;
        }
    }
}