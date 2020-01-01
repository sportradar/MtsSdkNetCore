/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    /// <summary>
    /// Defines a contract for channel settings
    /// </summary>
    internal interface IRabbitMqChannelSettings
    {
        /// <summary>
        /// Gets a value indicating whether the queue should be deleted on close
        /// </summary>
        bool DeleteQueueOnClose { get; }

        /// <summary>
        /// Gets a value indicating whether created queue is durable
        /// </summary>
        bool QueueIsDurable { get; }

        /// <summary>
        /// Gets a value indicating whether user acknowledgment enabled on queue
        /// </summary>
        bool UserAcknowledgmentEnabled { get; }

        /// <summary>
        /// Specifies minimum allowed value of the inactivity value
        /// </summary>
        int HeartBeat { get; }

        /// <summary>
        /// The user acknowledgment batch limit for received messages
        /// </summary>
        int UserAcknowledgmentBatchLimit { get; }

        /// <summary>
        /// The user acknowledgment timeout in seconds for received messages
        /// </summary>
        int UserAcknowledgmentTimeoutInSeconds { get; }

        /// <summary>
        /// Gets the delivery mode of the publishing channel (persistent or non-persistent)
        /// </summary>
        bool UsePersistentDeliveryMode { get; }

        /// <summary>
        /// Gets the publish queue limit (0 - unlimited)
        /// </summary>
        int PublishQueueLimit { get; }

        /// <summary>
        /// Gets the timeout for items in publish queue
        /// </summary>
        /// <value>Default 15 seconds</value>
        int PublishQueueTimeoutInMs1 { get; }

        /// <summary>
        /// Gets the timeout for items in publish queue
        /// </summary>
        /// <value>Default 15 seconds</value>
        /// <remarks>Used only with normal ticket sender (for live and prematch)</remarks>
        int PublishQueueTimeoutInMs2 { get; }

        /// <summary>
        /// Gets the maximum publish queue timeout in ms
        /// </summary>
        /// <value>The maximum publish queue timeout in ms</value>
        int MaxPublishQueueTimeoutInMs { get; }

        /// <summary>
        /// Gets a value indicating whether the rabbit consumer channel should be exclusive
        /// </summary>
        bool ExclusiveConsumer { get; }
    }
}
