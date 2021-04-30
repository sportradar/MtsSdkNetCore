/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.API.Internal.Senders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.API.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="ITicketResponse"/>
    /// </summary>
    /// <seealso cref="ITicketResponse" />
    [Serializable]
    internal class TicketResponse : ITicketResponse
    {
        /// <summary>
        /// The ticket sender
        /// </summary>
        private readonly ITicketSender _ticketSender;
        /// <summary>
        /// Gets the ticket id
        /// </summary>
        /// <value>Unique ticket id (in the client's system)</value>
        public string TicketId { get; }
        /// <summary>
        /// Gets the status of the ticket submission
        /// </summary>
        /// <value>The status</value>
        public TicketAcceptance Status { get; }
        /// <summary>
        /// Gets the response reason
        /// </summary>
        /// <value>The reason</value>
        public IResponseReason Reason { get; }
        /// <summary>
        /// Gets the bet details
        /// </summary>
        /// <value>The bet details</value>
        public IEnumerable<IBetDetail> BetDetails { get; }
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
        /// <summary>
        /// Gets the response signature/hash (previous BetAcceptanceId)
        /// </summary>
        /// <value>The signature</value>
        public string Signature { get; }
        /// <summary>
        /// Gets the exchange rate used when converting currencies to EUR. Long multiplied by 10000 and rounded to a long value
        /// </summary>
        /// <value>The exchange rate</value>
        public long ExchangeRate { get; }
        /// <summary>
        /// Gets the additional information
        /// </summary>
        /// <value>The additional information</value>
        public IDictionary<string, string> AdditionalInfo { get; }
        /// <summary>
        /// Gets the automatic accepted odds
        /// </summary>
        /// <value>The automatic accepted odds</value>
        public IEnumerable<IAutoAcceptedOdds> AutoAcceptedOdds { get; }
        /// <summary>
        /// Gets the timestamp of ticket placement (UTC)
        /// </summary>
        /// <value>The timestamp</value>
        public DateTime Timestamp { get; }
        /// <summary>
        /// The original json
        /// </summary>
        private readonly string _originalJson;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketResponse"/> class
        /// </summary>
        /// <param name="ticketSender">The ticket sender</param>
        /// <param name="ticketId">The ticket identifier</param>
        /// <param name="status">The status</param>
        /// <param name="reason">The reason</param>
        /// <param name="betDetails">The bet details</param>
        /// <param name="correlationId">The correlation id</param>
        /// <param name="signature">The signature</param>
        /// <param name="exchangeRate">The exchange rate</param>
        /// <param name="version">The version</param>
        /// <param name="additionalInfo">The additional information</param>
        /// <param name="autoAcceptedOdds">Auto accepted odds</param>
        /// <param name="orgJson">The original json string received from the mts</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Approved")]
        public TicketResponse(ITicketSender ticketSender,
                              string ticketId,
                              TicketAcceptance status,
                              IResponseReason reason,
                              IEnumerable<IBetDetail> betDetails,
                              string correlationId,
                              string signature = null,
                              long exchangeRate = -1,
                              string version = TicketHelper.MtsTicketVersion,
                              IDictionary<string, string> additionalInfo = null,
                              IEnumerable<IAutoAcceptedOdds> autoAcceptedOdds = null,
                              string orgJson = null)
        {
            Guard.Argument(ticketId, nameof(ticketId)).Require(TicketHelper.ValidateTicketId(ticketId));
            Guard.Argument(version, nameof(version)).NotNull().NotEmpty();

            TicketId = ticketId;
            Status = status;
            Reason = reason;
            if (betDetails != null)
            {
                BetDetails = betDetails as IReadOnlyCollection<IBetDetail>;
            }
            Signature = signature;
            ExchangeRate = exchangeRate;
            Version = version;
            Timestamp = DateTime.UtcNow;
            CorrelationId = correlationId;
            AdditionalInfo = additionalInfo != null && additionalInfo.Any()
                                 ? additionalInfo
                                 : null;
            AutoAcceptedOdds = autoAcceptedOdds;
            _originalJson = orgJson;

            _ticketSender = ticketSender;
        }

        /// <summary>
        /// Acknowledges the specified mark accepted.
        /// </summary>
        /// <param name="markAccepted">if set to <c>true</c> [mark accepted]</param>
        /// <param name="bookmakerId">The sender identifier</param>
        /// <param name="code">The code</param>
        /// <param name="message">The message</param>
        /// <exception cref="NullReferenceException">Missing TicketSender. Can not be null</exception>
        public void Acknowledge(bool markAccepted, int bookmakerId, int code, string message)
        {
            if (_ticketSender == null)
            {
                throw new InvalidOperationException("Missing TicketSender. Can not be null.");
            }
            var ticketAck = new TicketAck(TicketId,
                                          bookmakerId,
                                          markAccepted ? TicketAckStatus.Accepted : TicketAckStatus.Rejected,
                                          code,
                                          message);
            _ticketSender.SendTicket(ticketAck);
        }

        /// <summary>
        /// Send acknowledgment back to MTS
        /// </summary>
        /// <param name="markAccepted">if set to <c>true</c> [mark accepted]</param>
        /// <exception cref="Exception">missing ticket in cache</exception>
        public void Acknowledge(bool markAccepted = true)
        {
            var sentTicket = (ITicket) _ticketSender.GetSentTicket(TicketId);
            if (sentTicket == null)
            {
                throw new InvalidOperationException("Missing ticket in cache");
            }
            Acknowledge(markAccepted, sentTicket.Sender.BookmakerId, Reason.Code, Reason.Message);
        }

        public string ToJson()
        {
            return _originalJson;
        }
    }
}
