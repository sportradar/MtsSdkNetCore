/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Concurrent;
using Dawn;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.EventArguments;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;

namespace Sportradar.MTS.SDK.API.Internal.Senders
{
    /// <summary>
    /// Base implementation of the <see cref="ITicketSender"/>
    /// </summary>
    /// <seealso cref="ITicketSender" />
    internal abstract class TicketSenderBase : ITicketSender
    {
        /// <summary>
        /// The execution log
        /// </summary>
        private readonly ILogger _executionLog = SdkLoggerFactory.GetLogger(typeof(TicketSenderBase));
        /// <summary>
        /// The feed log
        /// </summary>
        private readonly ILogger _feedLog = SdkLoggerFactory.GetLoggerForFeedTraffic(typeof(TicketSenderBase));

        /// <summary>
        /// Raised when the attempt to send ticket failed
        /// </summary>
        public event EventHandler<TicketSendFailedEventArgs> TicketSendFailed;

        /// <summary>
        /// The publisher channel
        /// </summary>
        private readonly IRabbitMqPublisherChannel _publisherChannel;
        /// <summary>
        /// The ticket cache
        /// </summary>
        private readonly ConcurrentDictionary<string, TicketCacheItem> _ticketCache;
        /// <summary>
        /// The MTS channel settings
        /// </summary>
        private readonly IMtsChannelSettings _mtsChannelSettings;
        /// <summary>
        /// The rabbit mq channel settings
        /// </summary>
        private readonly IRabbitMqChannelSettings _rabbitMqChannelSettings;

        /// <summary>
        /// The timer
        /// </summary>
        private readonly ITimer _timer;
        /// <summary>
        /// Indication if it is opened or not
        /// </summary>
        private bool _isOpened;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketSenderBase"/> class
        /// </summary>
        /// <param name="publisherChannel">The publisher channel</param>
        /// <param name="ticketCache">The ticket cache</param>
        /// <param name="mtsChannelSettings">The MTS channel settings</param>
        /// <param name="rabbitMqChannelSettings">Rabbit channel settings</param>
        protected TicketSenderBase(IRabbitMqPublisherChannel publisherChannel,
                                  ConcurrentDictionary<string, TicketCacheItem> ticketCache,
                                  IMtsChannelSettings mtsChannelSettings,
                                  IRabbitMqChannelSettings rabbitMqChannelSettings)
        {
            Guard.Argument(publisherChannel, nameof(publisherChannel)).NotNull();
            Guard.Argument(ticketCache, nameof(ticketCache)).NotNull();
            Guard.Argument(mtsChannelSettings, nameof(mtsChannelSettings)).NotNull();
            Guard.Argument(rabbitMqChannelSettings, nameof(rabbitMqChannelSettings)).NotNull();

            _publisherChannel = publisherChannel;
            _ticketCache = ticketCache;
            _mtsChannelSettings = mtsChannelSettings;
            _rabbitMqChannelSettings = rabbitMqChannelSettings;
            _publisherChannel.MqMessagePublishFailed += PublisherChannelOnMqMessagePublishFailed;

            _timer = new SdkTimer(new TimeSpan(0, 0, 0, 0, GetCacheTimeout(null)), new TimeSpan(0, 0, 10));
            _timer.Elapsed += OnTimerElapsed;
            _timer.FireOnce(new TimeSpan(0, 0, 0, 0, GetCacheTimeout(null)));
        }

        /// <summary>
        /// Handles the <see cref="E:TimerElapsed" /> event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data</param>
        private void OnTimerElapsed(object sender, EventArgs e)
        {
            try
            {
                DeleteExpiredCacheItems();
            }
            catch (Exception exception)
            {
                _executionLog.LogWarning("Error during cleaning TicketCache.", exception);
            }
            _timer.FireOnce(new TimeSpan(0, 0, 10));
        }

        /// <summary>
        /// Deletes the expired cache items
        /// </summary>
        private void DeleteExpiredCacheItems()
        {
            var expiredItems = _ticketCache.Where(t => t.Value.Timestamp < DateTime.Now.AddMilliseconds(-GetCacheTimeout(null)));
            foreach (var ei in expiredItems)
            {
                _ticketCache.TryRemove(ei.Key, out _);
            }
        }

        /// <summary>
        /// Gets the mapped dto json MSG
        /// </summary>
        /// <param name="sdkTicket">The SDK ticket</param>
        /// <returns>System.String</returns>
        protected abstract string GetMappedDtoJsonMsg(ISdkTicket sdkTicket);

        /// <summary>
        /// Gets the byte MSG
        /// </summary>
        /// <param name="sdkTicket">The SDK ticket</param>
        /// <returns>System.Byte[]</returns>
        private byte[] GetByteMsg(ISdkTicket sdkTicket)
        {
            var json = GetMappedDtoJsonMsg(sdkTicket);
            if (_feedLog.IsEnabled(LogLevel.Debug))
            {
                _feedLog.LogDebug($"Sending {sdkTicket.GetType().Name}: {json}");
            }
            else
            {
                _feedLog.LogInformation($"Sending {sdkTicket.GetType().Name} with ticketId: {sdkTicket.TicketId}.");
            }
            if (_executionLog.IsEnabled(LogLevel.Debug))
            {
                _executionLog.LogDebug($"Sending {sdkTicket.GetType().Name}: {json}");
            }
            else
            {
                _executionLog.LogInformation($"Sending {sdkTicket.GetType().Name} with ticketId: {sdkTicket.TicketId}.");
            }
            var msg = Encoding.UTF8.GetBytes(json);
            return msg;
        }

        /// <summary>
        /// Sends the ticket
        /// </summary>
        /// <param name="ticket">The <see cref="ISdkTicket"/> to be send</param>
        public void SendTicket(ISdkTicket ticket)
        {
            var msg = GetByteMsg(ticket);
            if (string.IsNullOrEmpty(ticket.CorrelationId))
            {
                _feedLog.LogWarning($"Ticket: {ticket.TicketId} is missing CorrelationId.");
            }

            var ticketCI = new TicketCacheItem(TicketHelper.GetTicketTypeFromTicket(ticket), ticket.TicketId, ticket.CorrelationId, _mtsChannelSettings.ReplyToRoutingKey, null, ticket);

            // we clear cache, since already sent ticket with the same ticketId are not used (example: sending ticket, ticketAck, ticketCancel, ticketCancelAck)
            if (_ticketCache.TryRemove(ticket.TicketId, out _))
            {
                _executionLog.LogDebug($"Removed already sent ticket from cache {ticket.TicketId}");
            }

            _ticketCache.TryAdd(ticket.TicketId, ticketCI);
            _publisherChannel.Publish(ticket.TicketId, msg: msg, routingKey: _mtsChannelSettings.PublishRoutingKey, correlationId: ticket.CorrelationId, replyRoutingKey: _mtsChannelSettings.ReplyToRoutingKey);
        }

        /// <summary>
        /// Gets the sent ticket
        /// </summary>
        /// <param name="ticketId">The ticket identifier</param>
        /// <returns>ISdkTicket</returns>
        public ISdkTicket GetSentTicket(string ticketId)
        {
            if (_ticketCache.TryRemove(ticketId, out var ci))
            {
                return TicketHelper.GetTicketInSpecificType(ci);
            }
            return null;
        }

        private void PublisherChannelOnMqMessagePublishFailed(object sender, MessagePublishFailedEventArgs messagePublishFailedEventArgs)
        {
            _executionLog.LogInformation($"Message publishing failed with correlationId: {messagePublishFailedEventArgs.CorrelationId}, errorMessage: {messagePublishFailedEventArgs.ErrorMessage}, routingKey: {messagePublishFailedEventArgs.RoutingKey}.");

            var ticketId = string.Empty;
            var ci = _ticketCache.Values.FirstOrDefault(f => f.CorrelationId == messagePublishFailedEventArgs.CorrelationId);
            if (!string.IsNullOrEmpty(ci?.TicketId))
            {
                ticketId = ci.TicketId;
            }
            var json = Encoding.UTF8.GetString(messagePublishFailedEventArgs.RawData.ToArray());

            var arg = new TicketSendFailedEventArgs(ticketId, json, messagePublishFailedEventArgs.ErrorMessage);
            TicketSendFailed?.Invoke(sender, arg);
        }

        /// <summary>
        /// Gets a value indicating whether the current instance is opened
        /// </summary>
        /// <value><c>true</c> if this instance is opened; otherwise, <c>false</c></value>
        public bool IsOpened => _isOpened;

        /// <summary>
        /// Opens the current instance
        /// </summary>
        public void Open()
        {
            _publisherChannel.Open();
            _isOpened = true;
        }

        /// <summary>
        /// Closes the current instance
        /// </summary>
        public void Close()
        {
            _publisherChannel.Close();
            _isOpened = false;
        }

        /// <summary>
        /// Gets the get cache timeout
        /// </summary>
        /// <value>The get cache timeout</value>
        public int GetCacheTimeout(ISdkTicket ticket)
        {
            if (ticket == null)
            {
                return _rabbitMqChannelSettings.MaxPublishQueueTimeoutInMs;
            }

            if (ticket is ITicket t && t.Selections.Any(a => a.Id.StartsWith("lcoo")))
            {
                return _rabbitMqChannelSettings.PublishQueueTimeoutInMs2;
            }
            return _rabbitMqChannelSettings.PublishQueueTimeoutInMs1;
        }
    }
}
