/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Internal;

namespace Sportradar.MTS.SDK.API.Internal
{
    internal class MtsChannelSettings : IMtsChannelSettings
    {
        //reoffer-cancel-2.0-schema.json has no response (cancel.reoffer)
        //account has to have cashout enabled and it works only for live events

        public string ChannelQueueName { get; }

        public string ExchangeName { get; }

        public ExchangeType ExchangeType { get; }

        public IEnumerable<string> RoutingKeys { get; }

        public IReadOnlyDictionary<string, object> HeaderProperties { get; }

        public string ReplyToRoutingKey { get; }

        public string ConsumerTag { get; }

        public string PublishRoutingKey { get; }

        private MtsChannelSettings(string queueName, string exchangeName, ExchangeType exchangeType, string routingKey, IReadOnlyDictionary<string, object> headerProperties, string replyToRoutingKey, string environment)
        {
            ChannelQueueName = queueName;
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            RoutingKeys = string.IsNullOrEmpty(routingKey) ? null : new List<string> { routingKey };
            PublishRoutingKey = routingKey;
            HeaderProperties = headerProperties;
            ReplyToRoutingKey = replyToRoutingKey;
            var systemStartTime = DateTime.Now.AddMilliseconds(-Environment.TickCount);
            ConsumerTag = $"tag_{environment}|NETStd|{SdkInfo.GetVersion()}|{DateTime.Now:yyyyMMddHHmm}|{TicketHelper.DateTimeToUnixTime(systemStartTime)}|{Process.GetCurrentProcess().Id}";
        }

        internal MtsChannelSettings(string queueName, string exchangeName, ExchangeType exchangeType, IEnumerable<string> routingKeys, IReadOnlyDictionary<string, object> headerProperties, string environment)
        {
            ChannelQueueName = queueName;
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            if (routingKeys != null)
            {
                var enumerable = routingKeys as IList<string> ?? routingKeys.ToList();
                RoutingKeys = enumerable;
                PublishRoutingKey = enumerable.First();
            }
            HeaderProperties = headerProperties;
            var systemStartTime = DateTime.Now.AddMilliseconds(-Environment.TickCount);
            ConsumerTag = $"tag_{environment}|NET|{SdkInfo.GetVersion()}|{DateTime.Now:yyyyMMddHHmm}|{TicketHelper.DateTimeToUnixTime(systemStartTime)}|{Process.GetCurrentProcess().Id}";
        }

        public static IMtsChannelSettings GetTicketChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            var headers = new Dictionary<string, object> { { "replyRoutingKey", $"node{nodeId}.ticket.confirm" } };
            return new MtsChannelSettings(queueName: null,
                                          exchangeName: $"{rootExchangeName}-Submit",
                                          exchangeType: ExchangeType.Fanout,
                                          routingKey: $"{username}-Confirm-node{nodeId}",
                                          headerProperties: new ReadOnlyDictionary<string, object>(dictionary: headers),
                                          replyToRoutingKey: headers.First().Value.ToString(),
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketResponseChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            return new MtsChannelSettings(queueName: $"{username}-Confirm-node{nodeId}",
                                          exchangeName: $"{rootExchangeName}-Confirm",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: $"node{nodeId}.ticket.confirm",
                                          headerProperties: null,
                                          replyToRoutingKey: null,
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketAckChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            return new MtsChannelSettings(queueName: null,
                                          exchangeName: $"{rootExchangeName}-Ack",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: "ack.ticket",
                                          headerProperties: null,
                                          replyToRoutingKey: null,
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketCancelChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            var headers = new Dictionary<string, object> { { "replyRoutingKey", $"node{nodeId}.cancel.confirm" } };
            return new MtsChannelSettings(queueName: null,
                                          exchangeName: $"{rootExchangeName}-Control",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: "cancel",
                                          headerProperties: new ReadOnlyDictionary<string, object>(dictionary: headers),
                                          replyToRoutingKey: headers.First().Value.ToString(),
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketCancelResponseChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            return new MtsChannelSettings(queueName: $"{username}-Reply-node{nodeId}",
                                          exchangeName: $"{rootExchangeName}-Reply",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: $"node{nodeId}.cancel.confirm",
                                          headerProperties: null,
                                          replyToRoutingKey: null,
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketCancelAckChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            return new MtsChannelSettings(queueName: null,
                                          exchangeName: $"{rootExchangeName}-Ack",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: "ack.cancel",
                                          headerProperties: null,
                                          replyToRoutingKey: null,
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketReofferChannelSettings(string rootExchangeName, string username, int nodeId)
        {
            throw new InvalidProgramException("TicketReoffer must be send as normal ticket.");
        }

        public static IMtsChannelSettings GetTicketReofferCancelChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            return new MtsChannelSettings(queueName: null,
                                          exchangeName: $"{rootExchangeName}-Control",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: "cancel.reoffer",
                                          headerProperties: null, // new ReadOnlyDictionary<string, object>(dictionary: headers),
                                          replyToRoutingKey: null,
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketCashoutChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            var headers = new Dictionary<string, object> { { "replyRoutingKey", $"node{nodeId}.ticket.cashout" } };
            return new MtsChannelSettings(queueName: null,
                                          exchangeName: $"{rootExchangeName}-Control",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: "ticket.cashout",
                                          headerProperties: new ReadOnlyDictionary<string, object>(dictionary: headers),
                                          replyToRoutingKey: headers.First().Value.ToString(),
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketCashoutResponseChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            return new MtsChannelSettings(queueName: $"{username}-Reply-cashout-node{nodeId}",
                                          exchangeName: $"{rootExchangeName}-Reply",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: $"node{nodeId}.ticket.cashout",
                                          headerProperties: null,
                                          replyToRoutingKey: null,
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketNonSrSettleChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            var headers = new Dictionary<string, object> { { "replyRoutingKey", $"node{nodeId}.ticket.nonsrsettle" } };
            return new MtsChannelSettings(queueName: null,
                                          exchangeName: $"{rootExchangeName}-Control",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: "ticket.nonsrsettle",
                                          headerProperties: new ReadOnlyDictionary<string, object>(dictionary: headers),
                                          replyToRoutingKey: headers.First().Value.ToString(),
                                          environment: environment);
        }

        public static IMtsChannelSettings GetTicketNonSrSettleResponseChannelSettings(string rootExchangeName, string username, int nodeId, string environment)
        {
            return new MtsChannelSettings(queueName: $"{username}-Reply-nonsrsettle-node{nodeId}",
                                          exchangeName: $"{rootExchangeName}-Reply",
                                          exchangeType: ExchangeType.Topic,
                                          routingKey: $"node{nodeId}.ticket.nonsrsettle",
                                          headerProperties: null,
                                          replyToRoutingKey: null,
                                          environment: environment);
        }
    }
}
