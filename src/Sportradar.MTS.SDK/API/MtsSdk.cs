/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Dawn;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Formatters.Json.Extensions;
using App.Metrics.Meter;
using App.Metrics.Scheduling;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Exceptions;
using Sportradar.MTS.SDK.API.Internal;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;
using Sportradar.MTS.SDK.API.Internal.Senders;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Exceptions;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.EventArguments;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Unity;
using EntitiesMapper = Sportradar.MTS.SDK.API.Internal.EntitiesMapper;

namespace Sportradar.MTS.SDK.API
{
    /// <summary>
    /// A <see cref="IMtsSdk"/> implementation acting as an entry point to the MTS SDK
    /// </summary>
    public class MtsSdk : IMtsSdk
    {
        /// <summary>
        /// A log4net.ILog instance used for logging execution logs
        /// </summary>
        private readonly ILogger _executionLog;

        /// <summary>
        /// A log4net.ILog instance used for logging client iteration logs
        /// </summary>
        private readonly ILogger _interactionLog;

        /// <summary>
        /// A <see cref="ConnectionValidator"/> used to detect potential connectivity issues
        /// </summary>
        private readonly ConnectionValidator _connectionValidator;

        private readonly EntitiesMapper _entitiesMapper;
        private readonly ITicketSenderFactory _ticketPublisherFactory;

        private readonly IRabbitMqMessageReceiver _rabbitMqMessageReceiverForTickets;
        private readonly IRabbitMqMessageReceiver _rabbitMqMessageReceiverForTicketCancels;
        private readonly IRabbitMqMessageReceiver _rabbitMqMessageReceiverForTicketCashouts;
        private readonly IRabbitMqMessageReceiver _rabbitMqMessageReceiverForTicketNonSrSettle;

        private readonly ConcurrentDictionary<string, AutoResetEvent> _autoResetEventsForBlockingRequests;
        private readonly ConcurrentDictionary<string, ISdkTicket> _responsesFromBlockingRequests;
        private readonly MemoryCache _ticketsForNonBlockingRequests;
        private readonly object _lockForTicketsForNonBlockingRequestsCache;

        /// <summary>
        /// A <see cref="IUnityContainer"/> used to resolve
        /// </summary>
        private readonly IUnityContainer _unityContainer;

        /// <summary>
        /// Value indicating whether the instance has been disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Value indicating whether the current <see cref="IMtsSdk"/> is already opened
        /// 0 indicates false; 1 indicates true
        /// </summary>
        private long _isOpened;

        /// <summary>
        /// Gets a value indicating whether the current instance is opened
        /// </summary>
        public bool IsOpened => _isOpened == 1;

        /// <summary>
        /// The <see cref="ISdkConfiguration"/> representing sdk configuration
        /// </summary>
        private readonly ISdkConfigurationInternal _config;

        /// <summary>
        /// Raised when the current instance of <see cref="IMtsSdk" /> received <see cref="ITicketResponse" />
        /// </summary>
        public event EventHandler<TicketResponseReceivedEventArgs> TicketResponseReceived;

        /// <summary>
        /// Raised when the current instance of <see cref="IMtsSdk" /> did not receive <see cref="ITicketResponse" /> within timeout
        /// </summary>
        public event EventHandler<TicketMessageEventArgs> TicketResponseTimedOut;

        /// <summary>
        /// Raised when the attempt to send ticket failed
        /// </summary>
        public event EventHandler<TicketSendFailedEventArgs> SendTicketFailed;

        /// <summary>
        /// Raised when a message which cannot be parsed is received
        /// </summary>
        public event EventHandler<UnparsableMessageEventArgs> UnparsableTicketResponseReceived;

        /// <summary>
        /// Gets the <see cref="IBuilderFactory" /> instance used to construct builders with some
        /// of the properties pre-loaded from the configuration
        /// </summary>
        public IBuilderFactory BuilderFactory { get; }

        /// <summary>
        /// Gets the <see cref="IMtsClientApi"/> instance used to send requests to MTS REST API
        /// </summary>
        /// <value>The client api</value>
        public IMtsClientApi ClientApi { get; }

        /// <summary>
        /// Gets a <see cref="ICustomBetManager" /> instance used to perform various custom bet operations
        /// </summary>
        /// <value>The custom bet manager</value>
        public ICustomBetManager CustomBetManager { get; }

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <value>The connection status.</value>
        public IConnectionStatus ConnectionStatus { get; }

        /// <summary>
        /// Gets a <see cref="IReportManager" /> instance used to get various reports
        /// </summary>
        public IReportManager ReportManager { get; }

        /// <summary>
        /// A <see cref="IMetricsRoot"/> used to provide metrics within sdk
        /// </summary>
        private readonly IMetricsRoot _metricsRoot;
        /// <summary>
        /// A <see cref="AppMetricsTaskScheduler"/> used to schedule task to log sdk metrics
        /// </summary>
        private readonly AppMetricsTaskScheduler _metricsTaskScheduler;
        /// <summary>
        /// An <see cref="ILogger"/> used to log metrics
        /// </summary>
        private readonly ILogger _metricsLogger;

        /// <summary>
        /// Constructs a new instance of the <see cref="MtsSdk"/> class
        /// </summary>
        /// <param name="config">A <see cref="ISdkConfiguration"/> instance representing feed configuration</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> used to create <see cref="ILogger"/> used within sdk</param>
        /// <param name="metricsRoot">A <see cref="IMetricsRoot"/> used to provide metrics within sdk</param>
        public MtsSdk(ISdkConfiguration config, ILoggerFactory loggerFactory = null, IMetricsRoot metricsRoot = null)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            LogInit();

            _isDisposed = false;
            _isOpened = 0;

            _unityContainer = new UnityContainer();
            _unityContainer.RegisterTypes(config, loggerFactory, metricsRoot);
            _config = _unityContainer.Resolve<ISdkConfigurationInternal>();

            _executionLog = SdkLoggerFactory.GetLoggerForExecution(typeof(MtsSdk));
            _interactionLog = SdkLoggerFactory.GetLoggerForExecution(typeof(MtsSdk));

            LogInit();

            _metricsRoot = _unityContainer.Resolve<IMetricsRoot>();
            _metricsLogger = SdkLoggerFactory.GetLoggerForStats(typeof(MtsSdk));
            _metricsTaskScheduler = new AppMetricsTaskScheduler(
                TimeSpan.FromSeconds(_config.StatisticsTimeout),
                async () => { await LogMetricsAsync(); });
            

            _connectionValidator = _unityContainer.Resolve<ConnectionValidator>();

            BuilderFactory = _unityContainer.Resolve<IBuilderFactory>();
            _ticketPublisherFactory = _unityContainer.Resolve<ITicketSenderFactory>();

            _entitiesMapper = _unityContainer.Resolve<EntitiesMapper>();

            _rabbitMqMessageReceiverForTickets = _unityContainer.Resolve<IRabbitMqMessageReceiver>("TicketResponseMessageReceiver");
            _rabbitMqMessageReceiverForTicketCancels = _unityContainer.Resolve<IRabbitMqMessageReceiver>("TicketCancelResponseMessageReceiver");
            _rabbitMqMessageReceiverForTicketCashouts = _unityContainer.Resolve<IRabbitMqMessageReceiver>("TicketCashoutResponseMessageReceiver");
            _rabbitMqMessageReceiverForTicketNonSrSettle = _unityContainer.Resolve<IRabbitMqMessageReceiver>("TicketNonSrSettleResponseMessageReceiver");

            ClientApi = _unityContainer.Resolve<IMtsClientApi>();
            CustomBetManager = _unityContainer.Resolve<ICustomBetManager>();
            ConnectionStatus = _unityContainer.Resolve<IConnectionStatus>();
            ReportManager = _unityContainer.Resolve<IReportManager>();

            _autoResetEventsForBlockingRequests = new ConcurrentDictionary<string, AutoResetEvent>();
            _responsesFromBlockingRequests = new ConcurrentDictionary<string, ISdkTicket>();
            _ticketsForNonBlockingRequests = new MemoryCache("TicketsForNonBlockingRequests");
            _lockForTicketsForNonBlockingRequestsCache = new object();

            foreach (var t in Enum.GetValues(typeof(SdkTicketType)))
            {
                var publisher = _ticketPublisherFactory.GetTicketSender((SdkTicketType)t);
                if (publisher != null)
                {
                    publisher.TicketSendFailed += PublisherOnTicketSendFailed;
                }
            }
        }

        private void PublisherOnTicketSendFailed(object sender, TicketSendFailedEventArgs ticketSendFailedEventArgs)
        {
            _executionLog.LogInformation($"Publish of ticket {ticketSendFailedEventArgs.TicketId} failed.");
            // first clean it from awaiting ticket response
            lock (_lockForTicketsForNonBlockingRequestsCache)
            {
                if (_ticketsForNonBlockingRequests.Contains(ticketSendFailedEventArgs.TicketId))
                {
                    _ticketsForNonBlockingRequests.Remove(ticketSendFailedEventArgs.TicketId);
                }
            }
            SendTicketFailed?.Invoke(sender, ticketSendFailedEventArgs);
        }

        /// <summary>
        /// Constructs a <see cref="ISdkConfiguration" /> instance with information read from application configuration file
        /// </summary>
        /// <returns>A <see cref="ISdkConfiguration" /> instance read from application configuration file</returns>
        /// <exception cref="InvalidOperationException">The configuration could not be loaded, or the requested section does not exist in the config file</exception>
        /// <exception cref="ConfigurationErrorsException">The section read from the configuration file is not valid</exception>
        public static ISdkConfiguration GetConfiguration()
        {
            var section = SdkConfigurationSection.GetSection();
            return new SdkConfiguration(section);
        }

        /// <summary>
        /// Creates the <see cref="ISdkConfigurationBuilder"/> for building the <see cref="ISdkConfiguration"/>
        /// </summary>
        /// <returns>A <see cref="ISdkConfigurationBuilder"/> to be used to create <see cref="ISdkConfiguration"/></returns>
        public static ISdkConfigurationBuilder CreateConfigurationBuilder()
        {
            return new SdkConfigurationBuilder();
        }

        /// <summary>
        /// Closes the current <see cref="IMtsSdk"/> instance and disposes resources used by it
        /// </summary>
        public void Close()
        {
            ((IDisposable)this).Dispose();
        }

        /// <summary>
        /// Disposes the current instance and resources associated with it
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the current instance and resources associated with it
        /// </summary>
        /// <param name="disposing">Value indicating whether the managed resources should also be disposed</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _rabbitMqMessageReceiverForTickets.MqMessageReceived -= OnMqMessageReceived;
            _rabbitMqMessageReceiverForTickets.Close();

            _rabbitMqMessageReceiverForTicketCancels.MqMessageReceived -= OnMqMessageReceived;
            _rabbitMqMessageReceiverForTicketCancels.Close();

            _rabbitMqMessageReceiverForTicketCashouts.MqMessageReceived -= OnMqMessageReceived;
            _rabbitMqMessageReceiverForTicketCashouts.Close();

            _rabbitMqMessageReceiverForTicketNonSrSettle.MqMessageReceived -= OnMqMessageReceived;
            _rabbitMqMessageReceiverForTicketNonSrSettle.Close();

            _ticketPublisherFactory.Close();

            _metricsTaskScheduler.Dispose();

            foreach (var item in _autoResetEventsForBlockingRequests)
            {
                _autoResetEventsForBlockingRequests.TryRemove(item.Key, out var arEvent);
                _executionLog.LogDebug($"Disposing AutoResetEvent for TicketId: {item.Key}.");
                arEvent.Dispose();
            }

            _responsesFromBlockingRequests.Clear();

            if (disposing)
            {
                try
                {
                    _unityContainer.Dispose();
                }
                catch (Exception ex)
                {
                    _executionLog.LogWarning("An exception has occurred while disposing the feed instance. Exception: ", ex);
                }
            }

            _isDisposed = true;
            _isOpened = 0;
        }

        /// <summary>
        /// Opens the current feed
        /// </summary>
        /// <exception cref="ObjectDisposedException">The feed is already disposed</exception>
        /// <exception cref="InvalidOperationException">The feed is already opened</exception>
        /// <exception cref="CommunicationException"> Connection to the message broker failed, Probable Reason={Invalid or expired token}</exception>
        public void Open()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(ToString());
            }

            if (Interlocked.CompareExchange(ref _isOpened, 1, 0) != 0)
            {
                throw new InvalidOperationException("The feed is already opened");
            }

            try
            {
                _rabbitMqMessageReceiverForTickets.MqMessageReceived += OnMqMessageReceived;
                _rabbitMqMessageReceiverForTickets.Open();

                _rabbitMqMessageReceiverForTicketCancels.MqMessageReceived += OnMqMessageReceived;
                _rabbitMqMessageReceiverForTicketCancels.Open();

                _rabbitMqMessageReceiverForTicketCashouts.MqMessageReceived += OnMqMessageReceived;
                _rabbitMqMessageReceiverForTicketCashouts.Open();

                _rabbitMqMessageReceiverForTicketNonSrSettle.MqMessageReceived += OnMqMessageReceived;
                _rabbitMqMessageReceiverForTicketNonSrSettle.Open();

                _ticketPublisherFactory.Open();

                if (_config.StatisticsEnabled)
                {
                    _metricsTaskScheduler.Start();
                }
            }
            catch (CommunicationException ex)
            {
                // this should really almost never happen
                var result = _connectionValidator.ValidateConnection();
                if (result == ConnectionValidationResult.Success)
                {
                    throw new CommunicationException(
                        "Connection to the API failed, Probable Reason={Invalid or expired token}",
                        $"{_config.Host}:{_config.Port}",
                        ex.InnerException);
                }

                var publicIp = _connectionValidator.GetPublicIp();
                throw new CommunicationException(
                    $"Connection to the API failed. Probable Reason={result.Message}, Public IP={publicIp}",
                    $"{_config.Host}:{_config.Port}",
                    ex);
            }
            catch (BrokerUnreachableException ex)
            {
                // this should really almost never happen
                var result = _connectionValidator.ValidateConnection();
                if (result == ConnectionValidationResult.Success)
                {
                    throw new CommunicationException(
                        "Connection to the message broker failed, Probable Reason={Invalid or expired token}",
                        $"{_config.Host}:{_config.Port}",
                        ex.InnerException);
                }

                var publicIp = _connectionValidator.GetPublicIp();
                throw new CommunicationException(
                    $"Connection to the message broker failed. Probable Reason={result.Message}, Public IP={publicIp}",
                    $"{_config.Host}:{_config.Port}",
                    ex);
            }
            _executionLog.LogInformation("MtsSdk instance opened.");
        }

        private void OnMqMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
        {
            var stopwatch = Stopwatch.StartNew();

            if (_executionLog.IsEnabled(LogLevel.Debug))
            {
                _executionLog.LogDebug($"Received ticket response for correlationId={eventArgs.CorrelationId} and routingKey={eventArgs.RoutingKey}. JSON={eventArgs.JsonBody}");
            }
            else
            {
                _executionLog.LogInformation($"Received ticket response for correlationId={eventArgs.CorrelationId}.");
            }

            ISdkTicket ticket;
            try
            {
                ticket = _entitiesMapper.GetTicketResponseFromJson(eventArgs.JsonBody, eventArgs.RoutingKey, eventArgs.ResponseType, eventArgs.CorrelationId, eventArgs.AdditionalInfo);
                ((ConnectionStatus)ConnectionStatus).TicketReceived(ticket.TicketId);
            }
            catch (Exception e)
            {
                _executionLog.LogDebug("Received message deserialization failed.", e);
                //deserialization failed
                OnMqMessageDeserializationFailed(sender, new MessageDeserializationFailedEventArgs(Encoding.UTF8.GetBytes(eventArgs.JsonBody)));
                return;
            }

            // first clean it from awaiting ticket response
            lock (_lockForTicketsForNonBlockingRequestsCache)
            {
                if (_ticketsForNonBlockingRequests.Contains(ticket.TicketId))
                {
                    _ticketsForNonBlockingRequests.Remove(ticket.TicketId);
                }
            }

            // check if it was called from SendBlocking
            if (_autoResetEventsForBlockingRequests.ContainsKey(ticket.TicketId))
            {
                _responsesFromBlockingRequests.TryAdd(ticket.TicketId, ticket);
                ReleaseAutoResetEventFromDictionary(ticket.TicketId);
                return;
            }

            //else raise event
            var ticketReceivedEventArgs = new TicketResponseReceivedEventArgs(ticket);

            _metricsRoot.Measure.Meter.Mark(new MeterOptions{Context = "MtsSdk", Name = "TicketReceived", MeasurementUnit = Unit.Calls});

            _executionLog.LogInformation($"Invoking TicketResponseReceived event for {eventArgs.ResponseType} response with correlationId={eventArgs.CorrelationId}.");

            TicketResponseReceived?.Invoke(this, ticketReceivedEventArgs);

            stopwatch.Stop();
            _executionLog.LogInformation($"Processing TicketResponseReceived event for {eventArgs.ResponseType} response with correlationId={eventArgs.CorrelationId} finished in {stopwatch.ElapsedMilliseconds} ms.");
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnMqMessageDeserializationFailed(object sender, MessageDeserializationFailedEventArgs eventArgs)
        {
            var rawData = eventArgs.RawData as byte[] ?? eventArgs.RawData.ToArray();
            var basicMessageData = TicketHelper.ParseUnparsableMsg(rawData);
            _executionLog.LogInformation($"Extracted the following data from unparsed message data: [{basicMessageData}], raising OnUnparsableMessageReceived event");
            var dispatchEventArgs = new UnparsableMessageEventArgs(basicMessageData);
            _metricsRoot.Measure.Meter.Mark(new MeterOptions { Context = "MtsSdk", Name = "TicketDeserializationFailed", MeasurementUnit = Unit.Calls });
            UnparsableTicketResponseReceived?.Invoke(this, dispatchEventArgs);
        }

        /// <summary>
        /// Sends the ticket to the MTS server. The response will raise TicketResponseReceived event
        /// </summary>
        /// <param name="ticket">The <see cref="ISdkTicket"/> to be send</param>
        public void SendTicket(ISdkTicket ticket)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();

            _metricsRoot.Measure.Meter.Mark(new MeterOptions { Context = "MtsSdk", Name = "SendTicket", MeasurementUnit = Unit.Calls });
            _interactionLog.LogInformation($"Called SendTicket with ticketId={ticket.TicketId}.");
            SendTicketBase(ticket, false);
        }

        private int SendTicketBase(ISdkTicket ticket, bool waitForResponse)
        {
            if (!ConnectionStatus.IsConnected)
            {
                var msg = $"Sending {ticket.GetType().Name} with ticketId: {ticket.TicketId} failed. Reason: disconnected from server.";
                _executionLog.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }
            _executionLog.LogInformation($"Sending {ticket.GetType().Name} with ticketId: {ticket.TicketId}.");
            var ticketType = TicketHelper.GetTicketTypeFromTicket(ticket);
            var ticketSender = _ticketPublisherFactory.GetTicketSender(ticketType);
            ticketSender.SendTicket(ticket);
            var ticketCacheTimeout = ticketSender.GetCacheTimeout(ticket);
            if (waitForResponse)
            {
                return ticketCacheTimeout;
            }

            lock (_lockForTicketsForNonBlockingRequestsCache)
            {
                if (TicketResponseTimedOut != null)
                {
                    var cacheItemPolicyForTicketsForNonBlockingRequestsCache = new CacheItemPolicy
                    {
                        AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMilliseconds(ticketCacheTimeout)),
                        RemovedCallback = RemovedFromCacheForTicketsForNonBlockingRequestsCallback
                    };
                    _ticketsForNonBlockingRequests.Add(ticket.TicketId, ticket, cacheItemPolicyForTicketsForNonBlockingRequestsCache);
                }
            }

            return -1;
        }

        /// <summary>
        /// Sends the ticket to the MTS server and wait for the response message on the feed
        /// </summary>
        /// <param name="ticket">The <see cref="ITicket"/> to be send</param>
        /// <returns>Returns a <see cref="ITicketResponse" /></returns>
        public ITicketResponse SendTicketBlocking(ITicket ticket)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();

            _metricsRoot.Measure.Meter.Mark(new MeterOptions { Context = "MtsSdk", Name = "SendTicketBlocking", MeasurementUnit = Unit.Calls });
            _interactionLog.LogInformation($"Called SendTicketBlocking with ticketId={ticket.TicketId}.");
            return (ITicketResponse)SendTicketBlockingBase(ticket);
        }

        private ISdkTicket SendTicketBlockingBase(ISdkTicket ticket)
        {
            var stopwatch = Stopwatch.StartNew();
            var responseTimeout = SendTicketBase(ticket, true);

            var autoResetEvent = new AutoResetEvent(false);
            _autoResetEventsForBlockingRequests.TryAdd(ticket.TicketId, autoResetEvent);

            autoResetEvent.WaitOne(TimeSpan.FromMilliseconds(responseTimeout));

            if (_responsesFromBlockingRequests.TryRemove(ticket.TicketId, out var responseTicket))
            {
                stopwatch.Stop();
                ReleaseAutoResetEventFromDictionary(ticket.TicketId);
                _executionLog.LogDebug($"Sending in blocking mode and successfully received response for {ticket.GetType().Name} {ticket.TicketId} took {stopwatch.ElapsedMilliseconds} ms.");
                return responseTicket;
            }
            stopwatch.Stop();
            ReleaseAutoResetEventFromDictionary(ticket.TicketId);
            _executionLog.LogDebug($"Sending in blocking mode and waiting for receiving response for {ticket.GetType().Name} {ticket.TicketId} took {stopwatch.ElapsedMilliseconds} ms. Response not received in required timeout.");
            var msg = $"The timeout for receiving response elapsed. Org. {ticket.GetType().Name}: {ticket.TicketId}.";
            _executionLog.LogInformation(msg);
            throw new TimeoutException(msg);
        }

        private void RemovedFromCacheForTicketsForNonBlockingRequestsCallback(CacheEntryRemovedArguments arguments)
        {
            if (arguments.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var sendTicket = (ISdkTicket)arguments.CacheItem.Value;
                TicketResponseTimedOut?.Invoke(this, new TicketMessageEventArgs(sendTicket.TicketId, sendTicket, "Ticket response not received within timeout."));
            }
        }

        private void ReleaseAutoResetEventFromDictionary(string ticketId)
        {
            if (_autoResetEventsForBlockingRequests.TryRemove(ticketId, out var autoResetEvent))
            {
                autoResetEvent.Set();
                autoResetEvent.Dispose();
            }
        }

        /// <summary>
        /// Sends the cancel ticket to the MTS server and wait for the response message on the feed
        /// </summary>
        /// <param name="ticket">The <see cref="ITicketCancel"/> to be send</param>
        /// <returns>Returns a <see cref="ITicketCancelResponse" /></returns>
        public ITicketCancelResponse SendTicketCancelBlocking(ITicketCancel ticket)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();

            _metricsRoot.Measure.Meter.Mark(new MeterOptions { Context = "MtsSdk", Name = "SendTicketCancelBlocking", MeasurementUnit = Unit.Calls });
            _interactionLog.LogInformation($"Called SendTicketCancelBlocking with ticketId={ticket.TicketId}.");
            return (ITicketCancelResponse) SendTicketBlockingBase(ticket);
        }

        /// <summary>
        /// Sends the cashout ticket to the MTS server and wait for the response message on the feed
        /// </summary>
        /// <param name="ticket">A <see cref="ITicketCashout" /> to be send</param>
        /// <returns>Returns a <see cref="ITicketCashoutResponse" /></returns>
        public ITicketCashoutResponse SendTicketCashoutBlocking(ITicketCashout ticket)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();

            _metricsRoot.Measure.Meter.Mark(new MeterOptions { Context = "MtsSdk", Name = "SendTicketCashoutBlocking", MeasurementUnit = Unit.Calls });
            _interactionLog.LogInformation($"Called SendTicketCashoutBlocking with ticketId={ticket.TicketId}.");
            return (ITicketCashoutResponse)SendTicketBlockingBase(ticket);
        }

        /// <summary>
        /// Sends the cashout ticket to the MTS server and wait for the response message on the feed
        /// </summary>
        /// <param name="ticket">A <see cref="ITicketNonSrSettle" /> to be send</param>
        /// <returns>Returns a <see cref="ITicketNonSrSettleResponse" /></returns>
        public ITicketNonSrSettleResponse SendTicketNonSrSettleBlocking(ITicketNonSrSettle ticket)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();

            _metricsRoot.Measure.Meter.Mark(new MeterOptions { Context = "MtsSdk", Name = "SendTicketNonSrSettleBlocking", MeasurementUnit = Unit.Calls });
            _interactionLog.LogInformation($"Called SendTicketNonSrSettleBlocking with ticketId={ticket.TicketId}.");
            return (ITicketNonSrSettleResponse)SendTicketBlockingBase(ticket);
        }

        private static void LogInit()
        {
            var msg = "MtsSdk initialization. Version: " + SdkInfo.GetVersion();
            var logger = SdkLoggerFactory.GetLoggerForFeedTraffic(typeof(MtsSdk));
            logger.LogInformation(msg);
            logger = SdkLoggerFactory.GetLoggerForCache(typeof(MtsSdk));
            logger.LogInformation(msg);
            logger = SdkLoggerFactory.GetLoggerForClientInteraction(typeof(MtsSdk));
            logger.LogInformation(msg);
            logger = SdkLoggerFactory.GetLoggerForRestTraffic(typeof(MtsSdk));
            logger.LogInformation(msg);
            logger = SdkLoggerFactory.GetLoggerForStats(typeof(MtsSdk));
            logger.LogInformation(msg);
            logger = SdkLoggerFactory.GetLoggerForExecution(typeof(MtsSdk));
            logger.LogInformation(msg);
        }

        private Task LogMetricsAsync()
        {
            if (!_config.StatisticsEnabled)
            {
                _executionLog.LogDebug("Metrics logging is not enabled.");
                return Task.FromResult(false);
            }

            var snapshot = _metricsRoot.Snapshot.Get();
            snapshot.ToMetric();

            foreach (var formatter in _metricsRoot.OutputMetricsFormatters)
            {
                using var stream = new MemoryStream();
                formatter.WriteAsync(stream, snapshot);
                var result = Encoding.UTF8.GetString(stream.ToArray());
                _metricsLogger.LogInformation(result);
            }
            return Task.FromResult(true);
        }
    }
}
