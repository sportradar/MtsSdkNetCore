/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.EventArguments
{
    /// <summary>
    /// An event argument used by events raised when a message from the feed is received
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a <see cref="string"/> representing deserialized message
        /// </summary>
        public string JsonBody { get; }

        /// <summary>
        /// Gets the routing key of the received message
        /// </summary>
        public string RoutingKey { get; }

        /// <summary>
        /// Gets the correlation id of the received message
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        /// Gets the expected type of the response.
        /// </summary>
        /// <value>The type of the response</value>
        public TicketResponseType ResponseType { get; }

        /// <summary>
        /// Gets the additional information
        /// </summary>
        /// <value>The additional information</value>
        public IDictionary<string, string> AdditionalInfo { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class
        /// </summary>
        /// <param name="body">A JSON string representing the received message</param>
        /// <param name="routingKey">A routing key</param>
        /// <param name="correlationId">A correlation id</param>
        /// <param name="expectedTicketResponseType">Expected ticket response type</param>
        /// <param name="additionalInfo">Additional information</param>
        public MessageReceivedEventArgs(string body, string routingKey, string correlationId, TicketResponseType expectedTicketResponseType, IDictionary<string, string> additionalInfo)
        {
            Guard.Argument(body, nameof(body)).NotNull().NotEmpty();

            JsonBody = body;
            RoutingKey = routingKey;
            CorrelationId = correlationId;
            ResponseType = expectedTicketResponseType;
            AdditionalInfo = additionalInfo;
        }
    }
}