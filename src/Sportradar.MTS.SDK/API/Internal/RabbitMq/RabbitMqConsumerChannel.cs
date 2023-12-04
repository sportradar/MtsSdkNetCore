/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Internal;

namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    /// <summary>
    /// A class used to connect to the Rabbit MQ broker
    /// </summary>
    /// <seealso cref="IRabbitMqConsumerChannel" />
    internal class RabbitMqConsumerChannel : IRabbitMqConsumerChannel
    {
        /// <summary>
        /// Gets the unique identifier
        /// </summary>
        /// <value>The unique identifier</value>
        public int UniqueId { get; }

        /// <summary>
        /// The log4net.ILog used execution logging
        /// </summary>
        private static readonly ILogger ExecutionLog = SdkLoggerFactory.GetLogger(typeof(RabbitMqConsumerChannel));

        /// <summary>
        /// The feed log
        /// </summary>
        private static readonly ILogger FeedLog = SdkLoggerFactory.GetLoggerForFeedTraffic(typeof(RabbitMqConsumerChannel));

        /// <summary>
        /// A <see cref="IChannelFactory"/> used to construct the <see cref="IModel"/> representing Rabbit MQ channel
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
        /// Gets a value indicating whether the current <see cref="RabbitMqMessageReceiver"/> is currently opened;
        /// </summary>
        public bool IsOpened => Interlocked.Read(ref _isOpened) == 1;

        /// <summary>
        /// Occurs when the current channel received the data
        /// </summary>
        public event EventHandler<BasicDeliverEventArgs> ChannelMessageReceived;

        private readonly IMtsChannelSettings _mtsChannelSettings;

        private readonly IRabbitMqChannelSettings _channelSettings;

        private string _queueName;

        private IEnumerable<string> _routingKeys;

        /// <summary>
        /// The queue timer
        /// </summary>
        private readonly ITimer _healthTimer;

        private readonly int _timerInterval = 180;

        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqConsumerChannel"/> class
        /// </summary>
        /// <param name="channelFactory">A <see cref="IChannelFactory"/> used to construct the <see cref="IModel"/> representing Rabbit MQ channel</param>
        /// <param name="mtsChannelSettings"></param>
        /// <param name="channelSettings"></param>
        public RabbitMqConsumerChannel(IChannelFactory channelFactory,
                                       IMtsChannelSettings mtsChannelSettings,
                                       IRabbitMqChannelSettings channelSettings)
        {
            Guard.Argument(channelFactory, nameof(channelFactory)).NotNull();
            Guard.Argument(mtsChannelSettings, nameof(mtsChannelSettings)).NotNull();

            _channelFactory = channelFactory;
            _mtsChannelSettings = mtsChannelSettings;
            _channelSettings = channelSettings;

            _queueName = _mtsChannelSettings.ChannelQueueName;

            _shouldBeOpened = 0;

            UniqueId = _channelFactory.GetUniqueId();
            _healthTimer = new SdkTimer(new TimeSpan(0, 0, _timerInterval), new TimeSpan(0, 0, 1));
            _healthTimer.Elapsed += OnTimerElapsed;
        }

        /// <summary>
        /// Invoked when the internally used timer elapses
        /// </summary>
        /// <param name="sender">A <see cref="object" /> representation of the <see cref="ITimer" /> raising the event</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data</param>
        private void OnTimerElapsed(object sender, EventArgs e)
        {
            try
            {
                CreateAndOpenConsumerChannel();
            }
            catch (Exception)
            {
                // ignored
            }

            if (_shouldBeOpened == 1)
            {
                _healthTimer.FireOnce(TimeSpan.FromSeconds(_timerInterval));
            }
        }

        /// <summary>
        /// Handles the <see cref="EventingBasicConsumer.Received"/> event
        /// </summary>
        /// <param name="sender">The <see cref="object"/> representation of the instance raising the event</param>
        /// <param name="basicDeliverEventArgs">The <see cref="BasicDeliverEventArgs"/> instance containing the event data</param>
        private void OnDataReceived(object sender, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            var correlationId = basicDeliverEventArgs?.BasicProperties?.CorrelationId ?? string.Empty;
            FeedLog.LogInformation($"Received message from MTS with correlationId: {correlationId}.");
            ChannelMessageReceived?.Invoke(this, basicDeliverEventArgs);

            if (!_channelSettings.UserAcknowledgmentEnabled)
            {
                return;
            }

            var i = 0;
            while (i < 10)
            {
                i++;
                try
                {
                    var channelWrapper = _channelFactory.GetChannel(UniqueId);
                    CreateAndOpenConsumerChannel();
                    channelWrapper.Channel.BasicAck(basicDeliverEventArgs?.DeliveryTag ?? 0, false);
                    break;
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("unknown delivery tag"))
                    {
                        FeedLog.LogDebug($"Sending Ack for processed message {basicDeliverEventArgs?.DeliveryTag} failed. {e.Message}");
                    }
                    Thread.Sleep(i * 1000);
                }
            }
        }

        private void OnConsumerCanceled(object sender, ConsumerEventArgs e)
        {
            ExecutionLog.LogInformation($"Canceled consumer channel with channelNumber: {UniqueId} and queueName: {_queueName}.");
        }

        private void OnRegistered(object sender, ConsumerEventArgs e)
        {
            ExecutionLog.LogInformation($"Registered consumer channel with channelNumber: {UniqueId} and queueName: {_queueName}.");
        }

        private void OnUnregistered(object sender, ConsumerEventArgs e)
        {
            ExecutionLog.LogInformation($"Unregistered consumer channel with channelNumber: {UniqueId} and queueName: {_queueName}.");
        }

        private void OnShutdown(object sender, ShutdownEventArgs e)
        {
            ExecutionLog.LogInformation($"Shutdown consumer channel with channelNumber: {UniqueId} and queueName: {_queueName}. Reason={e.ReplyCode}-{e.ReplyText}, Cause={e.Cause}");
            Interlocked.CompareExchange(ref _isOpened, 0, 1);
            _healthTimer.FireOnce(TimeSpan.FromMilliseconds(100));
        }

        /// <summary>
        /// Opens the current channel and binds the created queue to provided routing keys
        /// </summary>
        public void Open()
        {
            Open(_mtsChannelSettings.ChannelQueueName, _mtsChannelSettings.RoutingKeys);
        }

        public void Open(IEnumerable<string> routingKeys)
        {
            Guard.Argument(routingKeys, nameof(routingKeys)).NotNull().NotEmpty();

            Open(_mtsChannelSettings.ChannelQueueName, routingKeys);
        }

        public void Open(string queueName, IEnumerable<string> routingKeys)
        {
            Guard.Argument(queueName, nameof(queueName)).NotNull().NotEmpty();
            Guard.Argument(routingKeys, nameof(routingKeys)).NotNull().NotEmpty();

            if (Interlocked.Read(ref _isOpened) == 1)
            {
                ExecutionLog.LogError("Opening an already opened channel is not allowed");
                throw new InvalidOperationException("The instance is already opened");
            }

            if (!string.IsNullOrEmpty(queueName))
            {
                _queueName = queueName;
            }

            _routingKeys = routingKeys;
            
            Interlocked.CompareExchange(ref _shouldBeOpened, 1, 0);
            CreateAndOpenConsumerChannel();
            _healthTimer.FireOnce(new TimeSpan(0, 0, _timerInterval));
        }

        private void CreateAndOpenConsumerChannel()
        {
            var sleepTime = 100;
            while (Interlocked.Read(ref _shouldBeOpened) == 1)
            {
                lock (_lock)
                {
                    try
                    {
                        var channelWrapper = _channelFactory.GetChannel(UniqueId);
                        if (CheckChannelWrapper(channelWrapper))
                        {
                            continue;
                        }

                        if (channelWrapper.Channel.IsOpen && channelWrapper.Consumer != null)
                        {
                            return;
                        }

                        ExecutionLog.LogInformation($"Opening the consumer channel with channelNumber: {UniqueId} and queueName: {_queueName}.");

                        // try to declare the exchange if it is not the default one
                        if (!string.IsNullOrEmpty(_mtsChannelSettings.ExchangeName))
                        {
                            MtsChannelSettings.TryDeclareExchange(channelWrapper.Channel, _mtsChannelSettings, _channelSettings.QueueIsDurable, ExecutionLog);
                        }

                        channelWrapper.Consumer = BindQueueAndCreateConsumer(channelWrapper.Channel);

                        Interlocked.CompareExchange(ref _isOpened, 1, 0);
                        return;
                    }
                    catch (Exception e)
                    {
                        ExecutionLog.LogInformation($"Error opening the consumer channel with channelNumber: {UniqueId} and queueName: {_queueName}.",
                            e);
                        if (e is IOException || e is AlreadyClosedException || e is SocketException)
                        {
                            sleepTime = SdkInfo.Increase(sleepTime, 500, 5000);
                        }
                        else
                        {
                            sleepTime = SdkInfo.Multiply(sleepTime, 1.25, 5000);
                        }

                        ExecutionLog.LogInformation($"Opening the consumer channel will be retried in next {sleepTime} ms.");
                        Thread.Sleep(sleepTime);
                    }
                }
            }
        }

        private bool CheckChannelWrapper(ChannelWrapper channelWrapper)
        {
            if (channelWrapper == null)
            {
                throw new OperationCanceledException("Missing consumer channel wrapper.");
            }

            if (channelWrapper.MarkedForDeletion)
            {
                _channelFactory.RemoveChannel(UniqueId);
                throw new OperationCanceledException("Consumer channel marked for deletion.");
            }

            if (channelWrapper.Channel.IsClosed && !channelWrapper.MarkedForDeletion)
            {
                channelWrapper.MarkedForDeletion = true;
                DisposeCurrentConsumer(channelWrapper);
                _channelFactory.RemoveChannel(UniqueId);
                return true;
            }

            return false;
        }

        private EventingBasicConsumer BindQueueAndCreateConsumer(IModel channel)
        {
            var arguments = new Dictionary<string, object> { { "x-queue-master-locator", "min-masters" } };

            var declareResult = _channelSettings.QueueIsDurable
                ? channel.QueueDeclare(_queueName, true, false, false, arguments)
                : channel.QueueDeclare(_queueName, false, false, false, arguments);

            if (!string.IsNullOrEmpty(_mtsChannelSettings.ExchangeName) && _routingKeys != null)
            {
                foreach (var routingKey in _routingKeys)
                {
                    ExecutionLog.LogInformation($"Binding queue={declareResult.QueueName} with routingKey={routingKey}");
                    channel.QueueBind(declareResult.QueueName,
                        exchange: _mtsChannelSettings.ExchangeName,
                        routingKey: routingKey);
                }
            }

            channel.BasicQos(0, 10, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnDataReceived;
            consumer.Registered += OnRegistered;
            consumer.Unregistered += OnUnregistered;
            consumer.Shutdown += OnShutdown;
            consumer.ConsumerCancelled += OnConsumerCanceled;
            channel.BasicConsume(queue: declareResult.QueueName, 
                autoAck: true,
                consumerTag: $"{_mtsChannelSettings.ConsumerTag}",
                consumer: consumer,
                noLocal: false,
                exclusive: _channelSettings.ExclusiveConsumer);
            return consumer;
        }

        /// <summary>
        /// Closes the current channel
        /// </summary>
        /// <exception cref="InvalidOperationException">The instance is already closed</exception>
        public void Close()
        {
            if (Interlocked.CompareExchange(ref _isOpened, 0, 1) != 1)
            {
                ExecutionLog.LogError($"Cannot close the consumer channel on channelNumber: {UniqueId}, because this channel is already closed.");
            }
            Interlocked.CompareExchange(ref _shouldBeOpened, 0, 1);
        }

        private void DisposeCurrentConsumer(ChannelWrapper channelWrapper)
        {
            if (channelWrapper?.Consumer == null)
            {
                return;
            }
            ExecutionLog.LogInformation($"Closing the consumer channel with channelNumber: {UniqueId} and queueName: {_queueName}.");
            channelWrapper.Consumer.Received -= OnDataReceived;
            channelWrapper.Consumer.Registered -= OnRegistered;
            channelWrapper.Consumer.Unregistered -= OnUnregistered;
            channelWrapper.Consumer.Shutdown -= OnShutdown;
            channelWrapper.Consumer.ConsumerCancelled -= OnConsumerCanceled;
            channelWrapper.Consumer = null;
        }
    }
}
