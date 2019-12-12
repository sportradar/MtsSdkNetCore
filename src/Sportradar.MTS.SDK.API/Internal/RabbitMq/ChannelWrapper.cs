/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    public class ChannelWrapper
    {
        public int Id { get; }

        public IModel Channel { get; }

        public EventingBasicConsumer Consumer { get; set; }

        public IBasicProperties ChannelBasicProperties { get; set; }

        public bool MarkedForDeletion { get; set; }

        public ChannelWrapper(int id, IModel channel)
        {
            Id = id;
            Channel = channel;
            Consumer = null;
            ChannelBasicProperties = null;
            MarkedForDeletion = false;
        }
    }
}
