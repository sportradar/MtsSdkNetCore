/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dawn;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.EventArguments;
using Sportradar.MTS.SDK.Entities.Internal;

namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    /// <summary>
    /// Implementation of <see cref="IRabbitMqPublisherChannel"/>
    /// </summary>
    /// <seealso cref="IRabbitMqPublisherChannel" />
    internal class RabbitMqPublisherChannel : IRabbitMqPublisherChannel
    {
        /// <summary>
        /// Gets the unique identifier
        /// </summary>
        /// <value>The unique identifier</value>
        public int UniqueId { get; }

        /// <summary>
        /// Raised when the attempt to publish message failed
        /// </summary>
        public event EventHandler<MessagePublishFailedEventArgs> MqMessagePublishFailed;

        /// <summary>
        /// The ILogger used execution logging
        /// </summary>
        private static readonly ILogger ExecutionLog = SdkLoggerFactory.GetLogger(typeof(RabbitMqPublisherChannel));

        /// <summary>
        /// The feed log
        /// </summary>
        private static readonly ILogger FeedLog = SdkLoggerFactory.GetLoggerForFeedTraffic(typeof(RabbitMqPublisherChannel));

        /// <summary>
        /// A <see cref="IChannelFactory" /> used to construct the <see cref="IModel" /> representing Rabbit MQ channel
        /// </summary>
        private readonly IChannelFactory _channelFactory;

        /// <summary>
        /// Value indicating whether the current instance is opened. 1 means opened, 0 means closed
        /// </summary>
        private long _isOpened;

        /// <summary>
        /// Value indicating whether the current instance should be opened. 1 means yes, 0 means no
        /// </summary>
        private long _shouldBeOpened;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="RabbitMqMessageReceiver" /> is currently opened;
        /// </summary>
        /// <value><c>true</c> if this instance is opened; otherwise, <c>false</c></value>
        public bool IsOpened => Interlocked.Read(ref _isOpened) == 1;

        /// <summary>
        /// The MTS channel settings
        /// </summary>
        private readonly IMtsChannelSettings _mtsChannelSettings;

        /// <summary>
        /// The channel settings
        /// </summary>
        private readonly IRabbitMqChannelSettings _channelSettings;

        /// <summary>
        /// The use queue
        /// </summary>
        private readonly bool _useQueue;

        /// <summary>
        /// The queue timeout
        /// </summary>
        private readonly int _queueTimeout;

        /// <summary>
        /// The queue limit
        /// </summary>
        private readonly int _queueLimit;

        /// <summary>
        /// The MSG queue
        /// </summary>
        private readonly ConcurrentQueue<PublishQueueItem> _msgQueue;

        /// <summary>
        /// The queue timer
        /// </summary>
        private readonly ITimer _queueTimer;

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <value>The connection status.</value>
        private readonly ConnectionStatus _connectionStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConsumerChannel" /> class
        /// </summary>
        /// <param name="channelFactory">A <see cref="IChannelFactory" /> used to construct the <see cref="IModel" /> representing Rabbit MQ channel</param>
        /// <param name="mtsChannelSettings">The mts channel settings</param>
        /// <param name="channelSettings">The channel settings</param>
        /// <param name="connectionStatus">The connection status</param>
        public RabbitMqPublisherChannel(IChannelFactory channelFactory, IMtsChannelSettings mtsChannelSettings, IRabbitMqChannelSettings channelSettings, IConnectionStatus connectionStatus)
        {
            Guard.Argument(channelFactory, nameof(channelFactory)).NotNull();
            Guard.Argument(mtsChannelSettings, nameof(mtsChannelSettings)).NotNull();
            Guard.Argument(channelSettings, nameof(channelSettings)).NotNull();
            Guard.Argument(connectionStatus, nameof(connectionStatus)).NotNull();

            _channelFactory = channelFactory;

            _mtsChannelSettings = mtsChannelSettings;

            _channelSettings = channelSettings;

            _useQueue = false;
            if (channelSettings.MaxPublishQueueTimeoutInMs > 0 || channelSettings.PublishQueueLimit > 0)
            {
                _useQueue = true;
                _msgQueue = new ConcurrentQueue<PublishQueueItem>();
                _queueLimit = channelSettings.PublishQueueLimit > 1 ? _channelSettings.PublishQueueLimit : -1;
                _queueTimeout = channelSettings.MaxPublishQueueTimeoutInMs >= 10000 ? _channelSettings.MaxPublishQueueTimeoutInMs : 15000;
                _queueTimer = new SdkTimer(new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 1));
                _queueTimer.Elapsed += OnTimerElapsed;
                _queueTimer.FireOnce(new TimeSpan(0, 0, 5));
            }
            _shouldBeOpened = 0;

            UniqueId = _channelFactory.GetUniqueId();

            _connectionStatus = (ConnectionStatus) connectionStatus;
        }

        /// <summary>
        /// Invoked when the internally used timer elapses
        /// </summary>
        /// <param name="sender">A <see cref="object" /> representation of the <see cref="ITimer" /> raising the event</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data</param>
        private void OnTimerElapsed(object sender, EventArgs e)
        {
            while (!_msgQueue.IsEmpty)
            {
                if (!_connectionStatus.IsConnected)
                {
                    break;
                }
                PublishQueueItem pqi = null;
                try
                {
                    if (_msgQueue.TryDequeue(out pqi))
                    {
                        //check if expired
                        if (pqi.Timestamp < DateTime.Now.AddMilliseconds(-_queueTimeout))
                        {
                            var msg = $"At {DateTime.Now} publishing queue item is expired. CorrelationId={pqi.CorrelationId}, RoutingKey={pqi.RoutingKey}, Added={pqi.Timestamp}.";
                            ExecutionLog.LogError(msg);
                            FeedLog.LogError(msg);
                            RaiseMessagePublishFailedEvent(pqi.Message, pqi.CorrelationId, pqi.RoutingKey, "Queue item is expired.");
                            continue;
                        }

                        //publish
                        var publishResult = PublishMsg(pqi.TicketId, (byte[]) pqi.Message, pqi.RoutingKey, pqi.CorrelationId, pqi.ReplyRoutingKey);

                        HandlePublishResult(publishResult, pqi);
                    }
                }
                catch (Exception exception)
                {
                    FeedLog.LogError($"Error during publishing queue item. CorrelationId={pqi?.CorrelationId}, RoutingKey={pqi?.RoutingKey}, Added={pqi?.Timestamp}.", exception);
                    if (pqi != null)
                    {
                        RaiseMessagePublishFailedEvent(pqi.Message, pqi.CorrelationId, pqi.RoutingKey, "Error during publishing queue item: " + exception);
                    }
                }
            }

            if (_useQueue)
            {
                _queueTimer.FireOnce(TimeSpan.FromMilliseconds(200)); // recheck after X milliseconds
            }
        }

        private void HandlePublishResult(IMqPublishResult publishResult, PublishQueueItem pqi)
        {
            if (publishResult.IsSuccess)
            {
                if (FeedLog.IsEnabled(LogLevel.Debug))
                {
                    FeedLog.LogDebug($"Publish succeeded. CorrelationId={pqi.CorrelationId}, RoutingKey={pqi.RoutingKey}, ReplyRoutingKey={pqi.ReplyRoutingKey}, Added={pqi.Timestamp}.");
                }
                else
                {
                    FeedLog.LogInformation($"Publish succeeded. CorrelationId={pqi.CorrelationId}, RoutingKey={pqi.RoutingKey}, Added={pqi.Timestamp}.");
                }
            }
            else
            {
                FeedLog.LogWarning($"Publish failed. CorrelationId={pqi.CorrelationId}, RoutingKey={pqi.RoutingKey}, Added={pqi.Timestamp}. Reason={publishResult.Message}");
                RaiseMessagePublishFailedEvent(pqi.Message, pqi.CorrelationId, pqi.RoutingKey, publishResult.Message);
            }
        }

        /// <summary>
        /// Raises the message publish failed event
        /// </summary>
        /// <param name="rawData">The raw data</param>
        /// <param name="correlationId">The correlation identifier</param>
        /// <param name="routingKey">The routing key</param>
        /// <param name="message">The message</param>
        private void RaiseMessagePublishFailedEvent(IEnumerable<byte> rawData, string correlationId, string routingKey, string message)
        {
            var args = new MessagePublishFailedEventArgs(rawData, correlationId, routingKey, message);
            MqMessagePublishFailed?.Invoke(this, args);
        }

        /// <summary>
        /// Publishes the specified message
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        /// <param name="msg">The message to be published</param>
        /// <param name="routingKey">The routing key to be set while publishing</param>
        /// <param name="correlationId">The correlation identifier</param>
        /// <param name="replyRoutingKey">The reply routing key</param>
        /// <returns>A <see cref="IMqPublishResult" /></returns>
        /// <exception cref="InvalidOperationException">The instance is closed</exception>
        public IMqPublishResult Publish(string ticketId, byte[] msg, string routingKey, string correlationId, string replyRoutingKey)
        {
            Guard.Argument(msg, nameof(msg)).NotNull();
            Guard.Argument(routingKey, nameof(routingKey)).NotNull().NotEmpty();

            if (_shouldBeOpened == 0)
            {
                throw new InvalidOperationException("The instance is closed");
            }
            if (!_connectionStatus.IsConnected)
            {
                throw new InvalidOperationException("Sending ticket failed. Reason: disconnected from server.");
            }
            if (_useQueue)
            {
                return AddToPublishingQueue(ticketId, msg, routingKey, correlationId, replyRoutingKey);
            }
            return PublishMsg(ticketId, msg, routingKey, correlationId, replyRoutingKey);
        }

        /// <summary>
        /// Publish message as an asynchronous operation
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        /// <param name="msg">The message to be published</param>
        /// <param name="routingKey">The routing key to be set while publishing</param>
        /// <param name="correlationId">The correlation identifier</param>
        /// <param name="replyRoutingKey">The reply routing key</param>
        /// <returns>A <see cref="IMqPublishResult" /></returns>
        public async Task<IMqPublishResult> PublishAsync(string ticketId, byte[] msg, string routingKey, string correlationId, string replyRoutingKey)
        {
            Guard.Argument(msg, nameof(msg)).NotNull();
            Guard.Argument(routingKey, nameof(routingKey)).NotNull().NotEmpty();

            return await Task.Run(() => Publish(ticketId, msg, routingKey, correlationId, replyRoutingKey)).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds to publishing queue
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        /// <param name="msg">The MSG</param>
        /// <param name="routingKey">The routing key</param>
        /// <param name="correlationId">The correlation identifier</param>
        /// <param name="replyRoutingKey">The reply routing key</param>
        /// <returns>IMqPublishResult</returns>
        private IMqPublishResult AddToPublishingQueue(string ticketId, byte[] msg, string routingKey, string correlationId, string replyRoutingKey)
        {
            var item = new PublishQueueItem(ticketId, msg, routingKey, correlationId, replyRoutingKey);

            if (_queueLimit > 0 && _msgQueue.Count >= _queueLimit)
            {
                var errorMessage = $"Publishing Queue is full. CorrelationId={correlationId}, RoutingKey={routingKey}.";
                FeedLog.LogError(errorMessage);
                ExecutionLog.LogError(errorMessage);
                //since user called Publish, we just return result and no need to call event handler
                return new MqPublishResult(correlationId, false, errorMessage);
            }

            _msgQueue.Enqueue(item);
            FeedLog.LogDebug($"Message with correlationId:{correlationId} and routingKey:{routingKey} added to publishing queue.");
            return new MqPublishResult(correlationId, true, "Item added to publishing queue.");
        }

        /// <summary>
        /// Publishes the MSG
        /// </summary>
        /// <param name="ticketId">Ticket Id</param>
        /// <param name="msg">The MSG</param>
        /// <param name="routingKey">The routing key</param>
        /// <param name="correlationId">The correlation identifier</param>
        /// <param name="replyRoutingKey">The reply routing key</param>
        /// <returns>IMqPublishResult</returns>
        /// <exception cref="InvalidOperationException">The instance is closed</exception>
        private IMqPublishResult PublishMsg(string ticketId, byte[] msg, string routingKey, string correlationId, string replyRoutingKey)
        {
            try
            {
                var channelWrapper = _channelFactory.GetChannel(UniqueId);
                CreateAndOpenPublisherChannel();
                if (channelWrapper.ChannelBasicProperties == null)
                {
                    throw new InvalidOperationException($"Channel {UniqueId} is not initiated.");
                }
                var channelBasicProperties = channelWrapper.ChannelBasicProperties;
                if (channelBasicProperties.Headers == null)
                {
                    channelBasicProperties.Headers = new Dictionary<string, object>();
                }
                if (!string.IsNullOrEmpty(correlationId))
                {
                    channelBasicProperties.CorrelationId = correlationId;
                }
                if (!string.IsNullOrEmpty(replyRoutingKey))
                {
                    channelBasicProperties.Headers["replyRoutingKey"] = replyRoutingKey;
                }
                channelWrapper.Channel.BasicPublish(_mtsChannelSettings.ExchangeName, routingKey, channelBasicProperties, msg);
                FeedLog.LogDebug($"Publish of message with correlationId:{correlationId} and routingKey:{routingKey} succeeded.");
                _connectionStatus.TicketSend(ticketId);
                return new MqPublishResult(correlationId);
            }
            catch (Exception e)
            {
                FeedLog.LogError($"Publish of message with correlationId:{correlationId} and routingKey:{routingKey} failed.", e);
                return new MqPublishResult(correlationId, false, e.Message);
            }
        }

        private void CreateAndOpenPublisherChannel()
        {
            var sleepTime = 1000;
            while (Interlocked.Read(ref _shouldBeOpened) == 1)
            {
                try
                {
                    var channelWrapper = _channelFactory.GetChannel(UniqueId);
                    if (CheckChannelWrapper(channelWrapper))
                    {
                        return;
                    }

                    channelWrapper.Channel.ModelShutdown += ChannelOnModelShutdown;
                    ExecutionLog.LogInformation($"Opening the publisher channel with channelNumber: {UniqueId} and exchangeName: {_mtsChannelSettings.ExchangeName}.");

                    // try to declare the exchange if it is not the default one
                    if (!string.IsNullOrEmpty(_mtsChannelSettings.ExchangeName))
                    {
                        MtsChannelSettings.TryDeclareExchange(channelWrapper.Channel, _mtsChannelSettings, _channelSettings.QueueIsDurable, ExecutionLog);
                    }

                    channelWrapper.ChannelBasicProperties = CreateBasicProperties(channelWrapper.Channel);

                    Interlocked.CompareExchange(ref _isOpened, 1, 0);
                    return;
                }
                catch (Exception e)
                {
                    ExecutionLog.LogInformation($"Error opening the publisher channel with channelNumber: {UniqueId} and exchangeName: {_mtsChannelSettings.ExchangeName}.", e);
                    if (e is IOException || e is AlreadyClosedException || e is SocketException)
                    {
                        sleepTime = SdkInfo.Increase(sleepTime, 500, 10000);
                    }
                    else
                    {
                        sleepTime = SdkInfo.Multiply(sleepTime, 1.25, _channelSettings.MaxPublishQueueTimeoutInMs * 1000);
                    }
                    ExecutionLog.LogInformation($"Opening the publisher channel will be retried in next {sleepTime} ms.");
                    Thread.Sleep(sleepTime);
                }
            }
        }

        /// <summary>
        /// Checks the channel wrapper
        /// </summary>
        /// <param name="channelWrapper">The channel wrapper</param>
        /// <returns><c>true</c> if should return from method, <c>false</c> otherwise.</returns>
        private bool CheckChannelWrapper(ChannelWrapper channelWrapper)
        {
            if (channelWrapper == null)
            {
                throw new OperationCanceledException("Missing publisher channel wrapper.");
            }
            if (channelWrapper.MarkedForDeletion)
            {
                throw new OperationCanceledException("Publisher channel marked for deletion.");
            }
            if (channelWrapper.Channel.IsOpen && channelWrapper.ChannelBasicProperties != null)
            {
                return true;
            }

            return false;
        }

        private IBasicProperties CreateBasicProperties(IModel channel)
        {
            var channelBasicProperties = channel.CreateBasicProperties();
            channelBasicProperties.ContentType = "application/json";
            channelBasicProperties.DeliveryMode = _channelSettings.UsePersistentDeliveryMode ? (byte)2 : (byte)1;

            //headerProperties like replyRoutingKey
            channelBasicProperties.Headers = new Dictionary<string, object>();
            if (_mtsChannelSettings.HeaderProperties != null && _mtsChannelSettings.HeaderProperties.Any())
            {
                foreach (var h in _mtsChannelSettings.HeaderProperties)
                {
                    channelBasicProperties.Headers.Add(h.Key, h.Value);
                }
            }

            return channelBasicProperties;
        }

        private void ChannelOnModelShutdown(object sender, ShutdownEventArgs e)
        {
            ExecutionLog.LogInformation($"Shutdown publisher channel with channelNumber: {UniqueId} and exchangeName: {_mtsChannelSettings.ExchangeName}. Reason={e.ReplyCode}-{e.ReplyText}, Cause={e.Cause}");
            Interlocked.CompareExchange(ref _isOpened, 0, 1);
        }

        /// <summary>
        /// Opens the current channel and binds the created queue to provided routing keys
        /// </summary>
        /// <exception cref="InvalidOperationException">The instance is already opened</exception>
        public void Open()
        {
            if (Interlocked.Read(ref _isOpened) != 0)
            {
                ExecutionLog.LogError("Opening an already opened channel is not allowed");
                throw new InvalidOperationException("The instance is already opened");
            }

            Interlocked.CompareExchange(ref _shouldBeOpened, 1, 0);
            CreateAndOpenPublisherChannel();
        }

        /// <summary>
        /// Closes the current channel
        /// </summary>
        /// <exception cref="InvalidOperationException">The instance is already closed</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1066:Collapsible \"if\" statements should be merged", Justification = "Approved for readability")]
        public void Close()
        {
            if (Interlocked.CompareExchange(ref _isOpened, 0, 1) != 1)
            {
                // Do not show error if the channel is scheduled to be open
                if (Interlocked.Read(ref _shouldBeOpened) != 1)
                {
                    ExecutionLog.LogError($"Cannot close the publisher channel on channelNumber: {UniqueId}, because this channel is already closed.");
                }
            }
            Interlocked.CompareExchange(ref _shouldBeOpened, 0, 1);
        }
    }
}
