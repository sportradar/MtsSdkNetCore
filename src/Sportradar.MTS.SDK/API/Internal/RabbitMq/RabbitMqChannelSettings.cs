/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using System.Diagnostics.CodeAnalysis;

namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    internal class RabbitMqChannelSettings : IRabbitMqChannelSettings
    {
        public bool DeleteQueueOnClose { get; }
        public bool QueueIsDurable { get; }
        public bool UserAcknowledgmentEnabled { get; }
        public int HeartBeat { get; }
        public int UserAcknowledgmentBatchLimit { get; }
        public int UserAcknowledgmentTimeoutInSeconds { get; }
        public bool UsePersistentDeliveryMode { get; }
        public int PublishQueueLimit { get; }
        public int PublishQueueTimeoutInMs1 { get; }
        public int PublishQueueTimeoutInMs2 { get; }
        public int MaxPublishQueueTimeoutInMs => PublishQueueTimeoutInMs1 >= PublishQueueTimeoutInMs2
                                                     ? PublishQueueTimeoutInMs1
                                                     : PublishQueueTimeoutInMs2;
        public bool ExclusiveConsumer { get; }

        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        public RabbitMqChannelSettings(bool queueDurable = false,
                                       bool exclusiveConsumer = true,
                                       bool enableUserAqs = false,
                                       bool deleteQueueOnClose = true,
                                       int heartBeat = 0,
                                       int ackBatchLimit = 1,
                                       int ackTimeout = 60,
                                       bool usePersistentDeliveryMode = false,
                                       int publishQueueLimit = 0,
                                       int publishQueueTimeoutInMs1 = 15000,
                                       int publishQueueTimeoutInMs2 = 15000)
        {
            DeleteQueueOnClose = deleteQueueOnClose;
            QueueIsDurable = queueDurable;
            UserAcknowledgmentEnabled = enableUserAqs;
            HeartBeat = heartBeat;
            UserAcknowledgmentBatchLimit = ackBatchLimit;
            UserAcknowledgmentTimeoutInSeconds = ackTimeout;
            UsePersistentDeliveryMode = usePersistentDeliveryMode;
            PublishQueueLimit = publishQueueLimit;
            PublishQueueTimeoutInMs1 = publishQueueTimeoutInMs1;
            PublishQueueTimeoutInMs2 = publishQueueTimeoutInMs2;
            ExclusiveConsumer = exclusiveConsumer;
        }
    }
}
