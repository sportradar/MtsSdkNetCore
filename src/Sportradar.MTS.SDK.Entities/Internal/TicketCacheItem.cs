/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    public class TicketCacheItem
    {
        public SdkTicketType TicketType { get; }

        public string TicketId { get; }

        public string CorrelationId { get; }

        public string ReplyRoutingKey { get; }

        public string ExchangeName { get; }

        public object Custom { get; }

        public DateTime Timestamp { get; }

        public TicketCacheItem(SdkTicketType type, string ticketId, string correlationId, string replyRoutingKey, string exchangeName, object custom = null)
        {
            TicketType = type;
            TicketId = ticketId;
            CorrelationId = correlationId;
            ReplyRoutingKey = replyRoutingKey;
            ExchangeName = exchangeName;
            Custom = custom;
            Timestamp = DateTime.Now;
        }
    }
}