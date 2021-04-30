/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Sportradar.MTS.SDK.API.Internal.Senders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.API.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="ITicketCancelResponse"/>
    /// </summary>
    /// <seealso cref="ITicketCancelResponse" />
    [Serializable]
    internal class TicketCancelResponse : ITicketCancelResponse
    {
        /// <summary>
        /// The ticket cancel sender
        /// </summary>
        private readonly ITicketSender _ticketCancelSender;
        /// <summary>
        /// Gets the ticket id
        /// </summary>
        /// <value>Unique ticket id (in the client's system)</value>
        public string TicketId { get; }
        /// <summary>
        /// Gets the response reason
        /// </summary>
        /// <value>The reason</value>
        public IResponseReason Reason { get; }
        /// <summary>
        /// Gets the status of the cancellation
        /// </summary>
        /// <value>The status</value>
        public TicketCancelAcceptance Status { get; }
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
        /// Gets the additional information about the response
        /// </summary>
        /// <value>The additional information</value>
        /// <remarks>Contains timestamps describing mts processing (receivedUtcTimestamp, validatedUtcTimestamp, respondedUtcTimestamp)</remarks>
        public IDictionary<string, string> AdditionalInfo { get; }
        /// <summary>
        /// Gets the timestamp of ticket placement (UTC)
        /// </summary>
        /// <value>The timestamp</value>
        public DateTime Timestamp { get; }

        private readonly string _originalJson;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCancelResponse"/> class
        /// </summary>
        /// <param name="ticketCancelSender">The ticket cancel sender</param>
        /// <param name="ticketId">The ticket identifier</param>
        /// <param name="status">The status</param>
        /// <param name="reason">The reason</param>
        /// <param name="correlationId">The correlation id</param>
        /// <param name="signature">The signature</param>
        /// <param name="version">The version</param>
        /// <param name="additionalInfo">The additional information</param>
        /// <param name="orgJson">The original json string received from the mts</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        public TicketCancelResponse(ITicketSender ticketCancelSender,
                                    string ticketId,
                                    TicketCancelAcceptance status,
                                    IResponseReason reason,
                                    string correlationId,
                                    string signature,
                                    string version = TicketHelper.MtsTicketVersion,
                                    IDictionary<string, string> additionalInfo = null,
                                    string orgJson = null)
        {
            TicketId = ticketId;
            Status = status;
            Reason = reason;
            Signature = signature;
            Version = version;
            Timestamp = DateTime.UtcNow;
            CorrelationId = correlationId;
            AdditionalInfo = additionalInfo;
            _originalJson = orgJson;

            _ticketCancelSender = ticketCancelSender;
        }

        /// <summary>
        /// Send acknowledgment back to MTS
        /// </summary>
        /// <param name="markAccepted">if set to <c>true</c> [mark canceled]</param>
        /// <param name="bookmakerId">The sender identifier (bookmakerId)</param>
        /// <param name="code">The code</param>
        /// <param name="message">The message</param>
        /// <exception cref="NullReferenceException">Missing TicketCancelSender. Can not be null</exception>
        public void Acknowledge(bool markAccepted, int bookmakerId, int code, string message)
        {
            if (_ticketCancelSender == null)
            {
                throw new InvalidOperationException("Missing TicketCancelSender. Can not be null.");
            }
            var ticketCancelAck = new TicketCancelAck(TicketId,
                                                      bookmakerId,
                                                      markAccepted ? TicketCancelAckStatus.Cancelled : TicketCancelAckStatus.NotCancelled,
                                                      code,
                                                      message);
            _ticketCancelSender.SendTicket(ticketCancelAck);
        }

        /// <summary>
        /// Send acknowledgment back to MTS
        /// </summary>
        /// <param name="markAccepted">if set to <c>true</c> [mark canceled]</param>
        /// <exception cref="Exception">missing ticket in cache</exception>
        public void Acknowledge(bool markAccepted = true)
        {
            var sentTicket = (ITicketCancel)_ticketCancelSender.GetSentTicket(TicketId);
            if (sentTicket == null)
            {
                throw new InvalidOperationException("Missing ticket in cache");
            }
            Acknowledge(markAccepted, sentTicket.BookmakerId, Reason.Code, Reason.Message);
        }

        public string ToJson()
        {
            return _originalJson;
        }
    }
}
