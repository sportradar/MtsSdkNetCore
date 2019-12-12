/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;

namespace Sportradar.MTS.SDK.Entities.EventArguments
{
    /// <summary>
    /// Event arguments for the MqMessagePublishFailed event
    /// </summary>
    public class MessagePublishFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="IEnumerable{Byte}"/> containing message data
        /// </summary>
        public IEnumerable<byte> RawData { get; }

        /// <summary>
        /// Gets the correlation id
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        /// Gets the routing key to which data should be send
        /// </summary>
        public string RoutingKey { get; }

        /// <summary>
        /// Gets the description of the error
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublishFailedEventArgs"/> class
        /// </summary>
        /// <param name="rawData">the name of the message which could not be deserialized, or a null reference if message name could
        ///                         not be retrieved</param>
        /// <param name="correlationId">The correlation id</param>
        /// <param name="routingKey">The routing key to which data should be send</param>
        /// <param name="message">The description of the error</param>
        public MessagePublishFailedEventArgs(IEnumerable<byte> rawData, string correlationId, string routingKey, string message)
        {
            Guard.Argument(rawData).NotNull().NotEmpty();

            RawData = rawData;
            CorrelationId = correlationId;
            RoutingKey = routingKey;
            ErrorMessage = message;
        }
    }
}