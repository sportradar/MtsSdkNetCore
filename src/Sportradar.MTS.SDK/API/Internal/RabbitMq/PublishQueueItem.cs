/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    internal class PublishQueueItem
    {
        public string TicketId { get; }

        public IEnumerable<byte> Message { get; }

        public string RoutingKey { get; }

        public string CorrelationId { get; }

        public string ReplyRoutingKey { get; }

        public object Custom { get; }

        public DateTime Timestamp { get; }

        public PublishQueueItem(string ticketId, byte[] msg, string routingKey, string correlationId, string replyRoutingKey, object custom = null)
        {
            TicketId = ticketId;
            Message = msg;
            RoutingKey = routingKey;
            CorrelationId = correlationId;
            ReplyRoutingKey = replyRoutingKey;
            Custom = custom;
            Timestamp = DateTime.Now;
        }
    }
}
