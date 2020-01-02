/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dawn;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using App.Metrics;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;
using Sportradar.MTS.SDK.API.Internal.Senders;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Common.Internal.Metrics;
using Sportradar.MTS.SDK.Common.Internal.Rest;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Builders;
using Sportradar.MTS.SDK.Entities.Internal.Cache;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

// ReSharper disable RedundantTypeArgumentsOfMethod

namespace Sportradar.MTS.SDK.API.Internal
{
    public static class SdkUnityBootstrapper
    {
        private static ILogger _log;
        private static IMetricsRoot _metricsRoot;
        private const int RestConnectionFailureLimit = 3;
        private const int RestConnectionFailureTimeoutInSec = 12;
        private static string _environment;
        private static readonly CultureInfo DefaultCulture = new CultureInfo("en");

        public static void RegisterTypes(this IUnityContainer container, ISdkConfiguration userConfig, ILoggerFactory loggerFactory, IMetricsRoot metricsRoot)
        {
            Guard.Argument(container, nameof(container)).NotNull();
            Guard.Argument(userConfig, nameof(userConfig)).NotNull();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (loggerFactory != null)
            {
                var _ = new SdkLoggerFactory(loggerFactory);
            }
            _log = SdkLoggerFactory.GetLogger(typeof(SdkUnityBootstrapper));

            if (metricsRoot == null)
            {
                _metricsRoot = new MetricsBuilder()
                    .Configuration.Configure(
                        options =>
                        {
                            options.DefaultContextLabel = "UF SDK .NET Core";
                            options.Enabled = true;
                            options.ReportingEnabled = true;
                        })
                    .OutputMetrics.AsPlainText()
                    .Build();
            }
            else
            {
                _metricsRoot = metricsRoot;
            }
            container.RegisterInstance(_metricsRoot, new ContainerControlledLifetimeManager());
            
            RegisterBaseClasses(container, userConfig);

            RegisterRabbitMqTypes(container, userConfig, _environment);

            RegisterTicketSenders(container);

            RegisterMarketDescriptionCache(container, userConfig);

            RegisterSdkStatisticsWriter(container);

            RegisterClientApi(container, userConfig);

            RegisterCustomBet(container);
        }

        private static void RegisterBaseClasses(IUnityContainer container, ISdkConfiguration config)
        {
            container.RegisterInstance(config, new ContainerControlledLifetimeManager());

            //register common types
            container.RegisterType<HttpClient, HttpClient>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor());

            var seed = (int)DateTime.Now.Ticks;
            var rand = new Random(seed);
            var value = rand.Next();
            _log.LogInformation($"Initializing sequence generator with MinValue={value}, MaxValue={long.MaxValue}");
            container.RegisterType<ISequenceGenerator, IncrementalSequenceGenerator>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    (long)value,
                    long.MaxValue));

            container.RegisterType<HttpDataFetcher, HttpDataFetcher>("Base",
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<HttpClient>(),
                    config.AccessToken ?? string.Empty,
                    RestConnectionFailureLimit,
                    RestConnectionFailureTimeoutInSec));

            container.RegisterType<LogHttpDataFetcher, LogHttpDataFetcher>("Base",
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<HttpClient>(),
                    config.AccessToken ?? string.Empty,
                    new ResolvedParameter<ISequenceGenerator>(),
                    RestConnectionFailureLimit,
                    RestConnectionFailureTimeoutInSec));

            var logFetcher = container.Resolve<LogHttpDataFetcher>("Base");
            container.RegisterInstance<IDataFetcher>("Base", logFetcher, new ContainerControlledLifetimeManager());
            container.RegisterInstance<IDataPoster>("Base", logFetcher, new ContainerControlledLifetimeManager());

            container.RegisterType<ISdkConfigurationInternal, SdkConfigurationInternal>(new ContainerControlledLifetimeManager());
            var configInternal = new SdkConfigurationInternal(config, logFetcher);
            container.RegisterInstance(configInternal);

            if (configInternal.Host.Contains("tradinggate"))
            {
                _environment = "PROD";
            }
            else if (configInternal.Host.Contains("integration"))
            {
                _environment = "CI";
            }
            else
            {
                _environment = "CUSTOM";
            }

            //container.RegisterType<IRabbitServer>(new ContainerControlledLifetimeManager());
            var rabbitServer = new RabbitServer(configInternal);
            container.RegisterInstance<IRabbitServer>(rabbitServer);

            container.RegisterType<ConnectionValidator, ConnectionValidator>(new ContainerControlledLifetimeManager());

            container.RegisterType<IConnectionFactory, ConfiguredConnectionFactory>(new ContainerControlledLifetimeManager());

            container.RegisterType<IChannelFactory, ChannelFactory>(new ContainerControlledLifetimeManager());

            container.RegisterInstance<ISequenceGenerator>(new IncrementalSequenceGenerator(), new ContainerControlledLifetimeManager());
        }

        private static void RegisterRabbitMqTypes(IUnityContainer container, ISdkConfiguration config, string environment)
        {
            container.RegisterType<IRabbitMqChannelSettings, RabbitMqChannelSettings>(new ContainerControlledLifetimeManager());
            container.RegisterInstance<IRabbitMqChannelSettings>("TicketChannelSettings", new RabbitMqChannelSettings(true, config.ExclusiveConsumer, publishQueueTimeoutInMs1: config.TicketResponseTimeoutLive, publishQueueTimeoutInMs2: config.TicketResponseTimeoutPrematch));
            container.RegisterInstance<IRabbitMqChannelSettings>("TicketCancelChannelSettings", new RabbitMqChannelSettings(true, config.ExclusiveConsumer, publishQueueTimeoutInMs1: config.TicketCancellationResponseTimeout, publishQueueTimeoutInMs2: config.TicketCancellationResponseTimeout));
            container.RegisterInstance<IRabbitMqChannelSettings>("TicketCashoutChannelSettings", new RabbitMqChannelSettings(true, config.ExclusiveConsumer, publishQueueTimeoutInMs1: config.TicketCashoutResponseTimeout, publishQueueTimeoutInMs2: config.TicketCashoutResponseTimeout));
            container.RegisterInstance<IRabbitMqChannelSettings>("TicketNonSrSettleChannelSettings", new RabbitMqChannelSettings(true, config.ExclusiveConsumer, publishQueueTimeoutInMs1: config.TicketNonSrSettleResponseTimeout, publishQueueTimeoutInMs2: config.TicketNonSrSettleResponseTimeout));

            var rootExchangeName = config.VirtualHost.Replace("/", string.Empty);
            container.RegisterType<IMtsChannelSettings, MtsChannelSettings>(new ContainerControlledLifetimeManager());
            var mtsTicketChannelSettings = MtsChannelSettings.GetTicketChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketCancelChannelSettings = MtsChannelSettings.GetTicketCancelChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketAckChannelSettings = MtsChannelSettings.GetTicketAckChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketCancelAckChannelSettings = MtsChannelSettings.GetTicketCancelAckChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketReofferCancelChannelSettings = MtsChannelSettings.GetTicketReofferCancelChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketCashoutChannelSettings = MtsChannelSettings.GetTicketCashoutChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketNonSrSettleChannelSettings = MtsChannelSettings.GetTicketNonSrSettleChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);

            var mtsTicketResponseChannelSettings = MtsChannelSettings.GetTicketResponseChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketCancelResponseChannelSettings = MtsChannelSettings.GetTicketCancelResponseChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketCashoutResponseChannelSettings = MtsChannelSettings.GetTicketCashoutResponseChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);
            var mtsTicketNonSrSettleResponseChannelSettings = MtsChannelSettings.GetTicketNonSrSettleResponseChannelSettings(rootExchangeName, config.Username, config.NodeId, environment);

            container.RegisterInstance("TicketChannelSettings", mtsTicketChannelSettings);
            container.RegisterInstance("TicketCancelChannelSettings", mtsTicketCancelChannelSettings);
            container.RegisterInstance("TicketAckChannelSettings", mtsTicketAckChannelSettings);
            container.RegisterInstance("TicketCancelAckChannelSettings", mtsTicketCancelAckChannelSettings);
            container.RegisterInstance("TicketReofferCancelChannelSettings", mtsTicketReofferCancelChannelSettings);
            container.RegisterInstance("TicketCashoutChannelSettings", mtsTicketCashoutChannelSettings);
            container.RegisterInstance("TicketNonSrSettleChannelSettings", mtsTicketNonSrSettleChannelSettings);

            container.RegisterInstance("TicketResponseChannelSettings", mtsTicketResponseChannelSettings);
            container.RegisterInstance("TicketCancelResponseChannelSettings", mtsTicketCancelResponseChannelSettings);
            container.RegisterInstance("TicketCashoutResponseChannelSettings", mtsTicketCashoutResponseChannelSettings);
            container.RegisterInstance("TicketNonSrSettleResponseChannelSettings", mtsTicketNonSrSettleResponseChannelSettings);

            //container.RegisterType<IRabbitMqConsumerChannel, RabbitMqConsumerChannel>(new HierarchicalLifetimeManager());
            var ticketResponseConsumerChannel = new RabbitMqConsumerChannel(container.Resolve<IChannelFactory>(),
                                                                            container.Resolve<IMtsChannelSettings>("TicketResponseChannelSettings"),
                                                                            container.Resolve<IRabbitMqChannelSettings>("TicketChannelSettings"));
            var ticketCancelResponseConsumerChannel = new RabbitMqConsumerChannel(container.Resolve<IChannelFactory>(),
                                                                            container.Resolve<IMtsChannelSettings>("TicketCancelResponseChannelSettings"),
                                                                            container.Resolve<IRabbitMqChannelSettings>("TicketCancelChannelSettings"));
            var ticketCashoutResponseConsumerChannel = new RabbitMqConsumerChannel(container.Resolve<IChannelFactory>(),
                                                                            container.Resolve<IMtsChannelSettings>("TicketCashoutResponseChannelSettings"),
                                                                            container.Resolve<IRabbitMqChannelSettings>("TicketCashoutChannelSettings"));
            var ticketNonSrSettleResponseConsumerChannel = new RabbitMqConsumerChannel(container.Resolve<IChannelFactory>(),
                                                                            container.Resolve<IMtsChannelSettings>("TicketNonSrSettleResponseChannelSettings"),
                                                                            container.Resolve<IRabbitMqChannelSettings>("TicketNonSrSettleChannelSettings"));
            container.RegisterInstance<IRabbitMqConsumerChannel>("TicketConsumerChannel", ticketResponseConsumerChannel);
            container.RegisterInstance<IRabbitMqConsumerChannel>("TicketCancelConsumerChannel", ticketCancelResponseConsumerChannel);
            container.RegisterInstance<IRabbitMqConsumerChannel>("TicketCashoutConsumerChannel", ticketCashoutResponseConsumerChannel);
            container.RegisterInstance<IRabbitMqConsumerChannel>("TicketNonSrSettleConsumerChannel", ticketNonSrSettleResponseConsumerChannel);

            container.RegisterType<IRabbitMqMessageReceiver, RabbitMqMessageReceiver>(new HierarchicalLifetimeManager());
            container.RegisterInstance<IRabbitMqMessageReceiver>("TicketResponseMessageReceiver", new RabbitMqMessageReceiver(ticketResponseConsumerChannel, TicketResponseType.Ticket));
            container.RegisterInstance<IRabbitMqMessageReceiver>("TicketCancelResponseMessageReceiver", new RabbitMqMessageReceiver(ticketCancelResponseConsumerChannel, TicketResponseType.TicketCancel));
            container.RegisterInstance<IRabbitMqMessageReceiver>("TicketCashoutResponseMessageReceiver", new RabbitMqMessageReceiver(ticketCashoutResponseConsumerChannel, TicketResponseType.TicketCashout));
            container.RegisterInstance<IRabbitMqMessageReceiver>("TicketNonSrSettleResponseMessageReceiver", new RabbitMqMessageReceiver(ticketNonSrSettleResponseConsumerChannel, TicketResponseType.TicketNonSrSettle));

            container.RegisterType<IRabbitMqPublisherChannel, RabbitMqPublisherChannel>(new HierarchicalLifetimeManager());
            var ticketPC = new RabbitMqPublisherChannel(container.Resolve<IChannelFactory>(),
                                                        mtsTicketChannelSettings,
                                                        container.Resolve<IRabbitMqChannelSettings>("TicketChannelSettings"));
            var ticketAckPC = new RabbitMqPublisherChannel(container.Resolve<IChannelFactory>(),
                                                        mtsTicketAckChannelSettings,
                                                        container.Resolve<IRabbitMqChannelSettings>("TicketChannelSettings"));
            var ticketCancelPC = new RabbitMqPublisherChannel(container.Resolve<IChannelFactory>(),
                                                        mtsTicketCancelChannelSettings,
                                                        container.Resolve<IRabbitMqChannelSettings>("TicketCancelChannelSettings"));
            var ticketCancelAckPC = new RabbitMqPublisherChannel(container.Resolve<IChannelFactory>(),
                                                        mtsTicketCancelAckChannelSettings,
                                                        container.Resolve<IRabbitMqChannelSettings>("TicketCancelChannelSettings"));
            var ticketReofferCancelPC = new RabbitMqPublisherChannel(container.Resolve<IChannelFactory>(),
                                                        mtsTicketReofferCancelChannelSettings,
                                                        container.Resolve<IRabbitMqChannelSettings>("TicketCancelChannelSettings"));
            var ticketCashoutPC = new RabbitMqPublisherChannel(container.Resolve<IChannelFactory>(),
                                                        mtsTicketCashoutChannelSettings,
                                                        container.Resolve<IRabbitMqChannelSettings>("TicketCashoutChannelSettings"));
            var ticketNonSrSettlePC = new RabbitMqPublisherChannel(container.Resolve<IChannelFactory>(),
                                                        mtsTicketNonSrSettleChannelSettings,
                                                        container.Resolve<IRabbitMqChannelSettings>("TicketNonSrSettleChannelSettings"));
            container.RegisterInstance<IRabbitMqPublisherChannel>("TicketPublisherChannel", ticketPC);
            container.RegisterInstance<IRabbitMqPublisherChannel>("TicketAckPublisherChannel", ticketAckPC);
            container.RegisterInstance<IRabbitMqPublisherChannel>("TicketCancelPublisherChannel", ticketCancelPC);
            container.RegisterInstance<IRabbitMqPublisherChannel>("TicketCancelAckPublisherChannel", ticketCancelAckPC);
            container.RegisterInstance<IRabbitMqPublisherChannel>("TicketReofferCancelPublisherChannel", ticketReofferCancelPC);
            container.RegisterInstance<IRabbitMqPublisherChannel>("TicketCashoutPublisherChannel", ticketCashoutPC);
            container.RegisterInstance<IRabbitMqPublisherChannel>("TicketNonSrSettlePublisherChannel", ticketNonSrSettlePC);
        }

        private static void RegisterTicketSenders(IUnityContainer container)
        {
            var ticketCache = new ConcurrentDictionary<string, TicketCacheItem>();

            //container.RegisterType<ITicketSender>(new ContainerControlledLifetimeManager());
            var ticketSender = new TicketSender(new TicketMapper(),
                                                container.Resolve<IRabbitMqPublisherChannel>("TicketPublisherChannel"),
                                                ticketCache,
                                                container.Resolve<IMtsChannelSettings>("TicketChannelSettings"),
                                                container.Resolve<IRabbitMqChannelSettings>("TicketChannelSettings"));
            var ticketAckSender = new TicketAckSender(new TicketAckMapper(),
                                                container.Resolve<IRabbitMqPublisherChannel>("TicketAckPublisherChannel"),
                                                ticketCache,
                                                container.Resolve<IMtsChannelSettings>("TicketAckChannelSettings"),
                                                container.Resolve<IRabbitMqChannelSettings>("TicketChannelSettings"));
            var ticketCancelSender = new TicketCancelSender(new TicketCancelMapper(),
                                                container.Resolve<IRabbitMqPublisherChannel>("TicketCancelPublisherChannel"),
                                                ticketCache,
                                                container.Resolve<IMtsChannelSettings>("TicketCancelChannelSettings"),
                                                container.Resolve<IRabbitMqChannelSettings>("TicketCancelChannelSettings"));
            var ticketCancelAckSender = new TicketCancelAckSender(new TicketCancelAckMapper(),
                                                container.Resolve<IRabbitMqPublisherChannel>("TicketCancelAckPublisherChannel"),
                                                ticketCache,
                                                container.Resolve<IMtsChannelSettings>("TicketCancelAckChannelSettings"),
                                                container.Resolve<IRabbitMqChannelSettings>("TicketCancelChannelSettings"));
            var ticketReofferCancelSender = new TicketReofferCancelSender(new TicketReofferCancelMapper(),
                                                container.Resolve<IRabbitMqPublisherChannel>("TicketReofferCancelPublisherChannel"),
                                                ticketCache,
                                                container.Resolve<IMtsChannelSettings>("TicketReofferCancelChannelSettings"),
                                                container.Resolve<IRabbitMqChannelSettings>("TicketCancelChannelSettings"));
            var ticketCashoutSender = new TicketCashoutSender(new TicketCashoutMapper(),
                                                container.Resolve<IRabbitMqPublisherChannel>("TicketCashoutPublisherChannel"),
                                                ticketCache,
                                                container.Resolve<IMtsChannelSettings>("TicketCashoutChannelSettings"),
                                                container.Resolve<IRabbitMqChannelSettings>("TicketCashoutChannelSettings"));
            var ticketNonSrSettleSender = new TicketNonSrSettleSender(new TicketNonSrSettleMapper(),
                                                container.Resolve<IRabbitMqPublisherChannel>("TicketNonSrSettlePublisherChannel"),
                                                ticketCache,
                                                container.Resolve<IMtsChannelSettings>("TicketNonSrSettleChannelSettings"),
                                                container.Resolve<IRabbitMqChannelSettings>("TicketNonSrSettleChannelSettings"));
            container.RegisterInstance<ITicketSender>("TicketSender", ticketSender);
            container.RegisterInstance<ITicketSender>("TicketAckSender", ticketAckSender);
            container.RegisterInstance<ITicketSender>("TicketCancelSender", ticketCancelSender);
            container.RegisterInstance<ITicketSender>("TicketCancelAckSender", ticketCancelAckSender);
            container.RegisterInstance<ITicketSender>("TicketReofferCancelSender", ticketReofferCancelSender);
            container.RegisterInstance<ITicketSender>("TicketCashoutSender", ticketCashoutSender);
            container.RegisterInstance<ITicketSender>("TicketNonSrSettleSender", ticketNonSrSettleSender);

            var senders = new Dictionary<SdkTicketType, ITicketSender>
            {
                {SdkTicketType.Ticket, container.Resolve<ITicketSender>("TicketSender")},
                {SdkTicketType.TicketAck, container.Resolve<ITicketSender>("TicketAckSender")},
                {SdkTicketType.TicketCancel, container.Resolve<ITicketSender>("TicketCancelSender")},
                {SdkTicketType.TicketCancelAck, container.Resolve<ITicketSender>("TicketCancelAckSender")},
                {SdkTicketType.TicketReofferCancel, container.Resolve<ITicketSender>("TicketReofferCancelSender")},
                {SdkTicketType.TicketCashout, container.Resolve<ITicketSender>("TicketCashoutSender")},
                {SdkTicketType.TicketNonSrSettle, container.Resolve<ITicketSender>("TicketNonSrSettleSender")},
            };

            var senderFactory = new TicketSenderFactory(senders);
            container.RegisterType<ITicketSenderFactory, TicketSenderFactory>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(senderFactory);

            var entityMapper = new EntitiesMapper(ticketAckSender, ticketCancelAckSender);
            container.RegisterType<EntitiesMapper>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(entityMapper);
        }

        private static void RegisterSdkStatisticsWriter(IUnityContainer container)
        {
            var x = container.ResolveAll<IRabbitMqMessageReceiver>();
            var statusProviders = new List<IHealthStatusProvider>();
            foreach (var mqMessageReceiver in x)
            {
                statusProviders.Add((IHealthStatusProvider) mqMessageReceiver);
            }

            //container.RegisterType<MetricsReporter, MetricsReporter>(new ContainerControlledLifetimeManager(),
            //                                                         new InjectionConstructor(MetricsReportPrintMode.Normal, 2, true));

            //var metricReporter = container.Resolve<MetricsReporter>();

            //Metric.Config.WithAllCounters().WithReporting(rep => rep.WithReport(metricReporter, TimeSpan.FromSeconds(config.StatisticsTimeout)));

            //container.RegisterInstance(metricReporter, new ContainerControlledLifetimeManager());

            foreach (var sp in statusProviders)
            {
                sp.RegisterHealthCheck();
            }
        }

        private static void RegisterMarketDescriptionCache(IUnityContainer container, ISdkConfiguration config)
        {
            var configInternal = container.Resolve<ISdkConfigurationInternal>();

            // Invariant market description provider
            container.RegisterType<IDeserializer<market_descriptions>, Deserializer<market_descriptions>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<market_descriptions, IEnumerable<MarketDescriptionDTO>>, MarketDescriptionsMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataProvider<IEnumerable<MarketDescriptionDTO>>,
                DataProvider<market_descriptions, IEnumerable<MarketDescriptionDTO>>>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    configInternal.ApiHost + "/v1/descriptions/{0}/markets.xml?include_mappings=true",
                    new ResolvedParameter<IDataFetcher>("Base"),
                    new ResolvedParameter<IDataPoster>("Base"),
                    new ResolvedParameter<IDeserializer<market_descriptions>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<market_descriptions, IEnumerable<MarketDescriptionDTO>>>()));

            // Cache for invariant markets
            container.RegisterInstance(
                "InvariantMarketDescriptionCache_Cache",
                new MemoryCache("invariantMarketsDescriptionsCache"),
                new ContainerControlledLifetimeManager());

            // Timer for invariant markets
            container.RegisterType<ITimer, SdkTimer>(
                "InvariantMarketCacheTimer",
                new HierarchicalLifetimeManager(),
                new InjectionConstructor(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromHours(6)));

            // Invariant market cache
            container.RegisterType<IMarketDescriptionCache, MarketDescriptionCache>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<MemoryCache>("InvariantMarketDescriptionCache_Cache"),
                    new ResolvedParameter<IDataProvider<IEnumerable<MarketDescriptionDTO>>>(),
                    new List<CultureInfo> { DefaultCulture },
                    config.AccessToken ?? string.Empty,
                    TimeSpan.FromHours(4),
                    new CacheItemPolicy { SlidingExpiration = TimeSpan.FromDays(1) },
                    new ResolvedParameter<IMetricsRoot>()));

            container.RegisterType<IMarketDescriptionProvider, MarketDescriptionProvider>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IMarketDescriptionCache>(),
                    new List<CultureInfo> { DefaultCulture }));

            container.RegisterType<IBuilderFactory, BuilderFactory>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<ISdkConfigurationInternal>(),
                    new ResolvedParameter<IMarketDescriptionProvider>()));
        }

        private static void RegisterClientApi(IUnityContainer container, ISdkConfiguration userConfig)
        {
            container.RegisterType<HttpDataFetcher, HttpDataFetcher>("MtsClientApi",
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<HttpClient>(),
                    string.Empty,
                    RestConnectionFailureLimit,
                    RestConnectionFailureTimeoutInSec));

            container.RegisterType<LogHttpDataFetcher, LogHttpDataFetcher>("MtsClientApi",
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<HttpClient>(),
                    string.Empty,
                    new ResolvedParameter<ISequenceGenerator>(),
                    RestConnectionFailureLimit,
                    RestConnectionFailureTimeoutInSec));

            var logFetcher = container.Resolve<LogHttpDataFetcher>("MtsClientApi");
            container.RegisterInstance<IDataFetcher>("MtsClientApi", logFetcher, new ContainerControlledLifetimeManager());
            container.RegisterInstance<IDataPoster>("MtsClientApi", logFetcher, new ContainerControlledLifetimeManager());

            container.RegisterType<IDeserializer<AccessTokenDTO>, Entities.Internal.JsonDeserializer<AccessTokenDTO>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<AccessTokenDTO, KeycloakAuthorization>, KeycloakAuthorizationMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataProvider<KeycloakAuthorization>,
                DataProvider<AccessTokenDTO, KeycloakAuthorization>>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    userConfig.KeycloakHost + "/auth/realms/mts/protocol/openid-connect/token",
                    new ResolvedParameter<IDataFetcher>("MtsClientApi"),
                    new ResolvedParameter<IDataPoster>("MtsClientApi"),
                    new ResolvedParameter<IDeserializer<AccessTokenDTO>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<AccessTokenDTO, KeycloakAuthorization>>()));

            container.RegisterType<IDeserializer<MaxStakeResponseDTO>, Entities.Internal.JsonDeserializer<MaxStakeResponseDTO>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<MaxStakeResponseDTO, MaxStakeImpl>, MaxStakeMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataProvider<MaxStakeImpl>,
                DataProvider<MaxStakeResponseDTO, MaxStakeImpl>>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    userConfig.MtsClientApiHost + "/ClientApi/api/maxStake/v1",
                    new ResolvedParameter<IDataFetcher>("MtsClientApi"),
                    new ResolvedParameter<IDataPoster>("MtsClientApi"),
                    new ResolvedParameter<IDeserializer<MaxStakeResponseDTO>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<MaxStakeResponseDTO, MaxStakeImpl>>()));

            container.RegisterType<IDeserializer<CcfResponseDTO>, Entities.Internal.JsonDeserializer<CcfResponseDTO>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<CcfResponseDTO, CcfImpl>, CcfMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataProvider<CcfImpl>,
                DataProvider<CcfResponseDTO, CcfImpl>>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    userConfig.MtsClientApiHost + "/ClientApi/api/ccf/v1?sourceId={0}",
                    new ResolvedParameter<IDataFetcher>("MtsClientApi"),
                    new ResolvedParameter<IDataPoster>("MtsClientApi"),
                    new ResolvedParameter<IDeserializer<CcfResponseDTO>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<CcfResponseDTO, CcfImpl>>()));

            container.RegisterType<IMtsClientApi, MtsClientApi>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IDataProvider<MaxStakeImpl>>(),
                    new ResolvedParameter<IDataProvider<CcfImpl>>(),
                    new ResolvedParameter<IDataProvider<KeycloakAuthorization>>(),
                    new InjectionParameter<string>(userConfig.KeycloakUsername),
                    new InjectionParameter<string>(userConfig.KeycloakPassword),
                    new InjectionParameter<string>(userConfig.KeycloakSecret),
                    new ResolvedParameter<IMetricsRoot>()
                ));
        }

        private static void RegisterCustomBet(IUnityContainer container)
        {
            var configInternal = container.Resolve<ISdkConfigurationInternal>();

            container.RegisterType<IDeserializer<AvailableSelectionsType>, Deserializer<AvailableSelectionsType>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<AvailableSelectionsType, AvailableSelectionsDTO>, AvailableSelectionsMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataProvider<AvailableSelectionsDTO>, DataProvider<AvailableSelectionsType, AvailableSelectionsDTO>>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    configInternal.ApiHost + "/v1/custombet/{0}/available_selections",
                    new ResolvedParameter<IDataFetcher>("Base"),
                    new ResolvedParameter<IDataPoster>("Base"),
                    new ResolvedParameter<IDeserializer<AvailableSelectionsType>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<AvailableSelectionsType, AvailableSelectionsDTO>>()));

            container.RegisterType<IDeserializer<CalculationResponseType>, Deserializer<CalculationResponseType>>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISingleTypeMapperFactory<CalculationResponseType, CalculationDTO>, CalculationMapperFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICalculateProbabilityProvider, CalculateProbabilityProvider>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    configInternal.ApiHost + "/v1/custombet/calculate",
                    new ResolvedParameter<IDataPoster>("Base"),
                    new ResolvedParameter<IDeserializer<CalculationResponseType>>(),
                    new ResolvedParameter<ISingleTypeMapperFactory<CalculationResponseType, CalculationDTO>>()));

            container.RegisterType<ICustomBetManager, CustomBetManager>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IDataProvider<AvailableSelectionsDTO>>(),
                    new ResolvedParameter<ICalculateProbabilityProvider>()));
        }
    }
}
