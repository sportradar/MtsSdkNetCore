/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="ITicketAck"/>
    /// </summary>
    /// <seealso cref="ITicketAck" />
    [Serializable]
    internal class TicketAck : ITicketAck
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
        /// Gets the status of the ticket
        /// </summary>
        /// <value>The ticket status</value>
        public TicketAckStatus TicketStatus { get; }
        /// <summary>
        /// Get the code
        /// </summary>
        /// <value>The code</value>
        public int Code { get; }
        /// <summary>
        /// Gets the ticket format version
        /// </summary>
        /// <value>The version</value>
        public string Version { get; }

        /// <summary>
        /// Gets the correlation identifier.
        /// </summary>
        /// <value>The correlation identifier</value>
        public string CorrelationId { get; }

        public string ToJson()
        {
            var dto = EntitiesMapper.Map(this);
            return dto.ToJson();
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message</value>
        public string Message { get; }

        [JsonConstructor]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        private TicketAck(DateTime timestamp, string ticketId, int bookmakerId, TicketAckStatus ticketStatus, int code, string version, string correlationId, string message)
        {
            Timestamp = timestamp;
            TicketId = ticketId;
            BookmakerId = bookmakerId;
            TicketStatus = ticketStatus;
            Code = code;
            Version = version;
            CorrelationId = correlationId;
            Message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketAck"/> class
        /// </summary>
        /// <param name="ticketId">The ticket identifier</param>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <param name="status">The status</param>
        /// <param name="code">The code</param>
        /// <param name="message">The message</param>
        public TicketAck(string ticketId, int bookmakerId, TicketAckStatus status, int code, string message)
        {
            Guard.Argument(ticketId, nameof(ticketId)).Require(TicketHelper.ValidateTicketId(ticketId));
            Guard.Argument(bookmakerId, nameof(bookmakerId)).Positive();

            TicketId = ticketId;
            BookmakerId = bookmakerId;
            Code = code;
            TicketStatus = status;
            Message = message;
            Timestamp = DateTime.Now;
            Version = TicketHelper.MtsTicketVersion;
            CorrelationId = TicketHelper.GenerateTicketCorrelationId();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketAck"/> class
        /// </summary>
        /// <param name="ticket">The ticket</param>
        /// <param name="status">The status</param>
        /// <param name="code">The code</param>
        /// <param name="message">The message</param>
        public TicketAck(ITicket ticket, TicketAckStatus status, int code, string message)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();

            TicketId = ticket.TicketId;
            BookmakerId = ticket.Sender.BookmakerId;
            Code =  code;
            TicketStatus = status;
            Message = message;
            Timestamp = DateTime.UtcNow;
            Version = TicketHelper.MtsTicketVersion;
            CorrelationId = TicketHelper.GenerateTicketCorrelationId();
        }
    }
}