/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;

namespace Sportradar.MTS.SDK.API.Internal
{
    /// <summary>
    /// Defines a contract for classes providing channel setting in accordance to MTS recommendations
    /// </summary>
    internal interface IMtsChannelSettings
    {
        /// <summary>
        /// Gets the name of the ticket binding exchange
        /// </summary>
        /// <value>The name of the ticket binding exchange</value>
        string ExchangeName { get; }

        /// <summary>
        /// The type of exchange on rabbit
        /// </summary>
        ExchangeType ExchangeType { get; }

        /// <summary>
        /// The name of the queue
        /// </summary>
        /// <value>The name of the queue</value>
        string ChannelQueueName { get; }

        /// <summary>
        /// The list of routing keys
        /// </summary>
        /// <value>The list of routing keys</value>
        IEnumerable<string> RoutingKeys { get; }

        /// <summary>
        /// The list of routing key for publishing
        /// </summary>
        /// <value>The list of routing key for publishing</value>
        string PublishRoutingKey { get; }

        /// <summary>
        /// the list of header properties
        /// </summary>
        IReadOnlyDictionary<string, object> HeaderProperties { get; }

        /// <summary>
        /// The name of the routingKey for reply
        /// </summary>
        /// <value>The name of the routingKey for reply</value>
        string ReplyToRoutingKey { get; }

        /// <summary>
        /// Gets the consumer tag which will be set on <see cref="IRabbitMqConsumerChannel"/>
        /// </summary>
        /// <value>The consumer tag</value>
        string ConsumerTag { get; }
    }
}