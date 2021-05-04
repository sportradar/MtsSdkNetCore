/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Represents SDK configuration
    /// </summary>
    /// <seealso cref="ISdkConfiguration" />
    internal class SdkConfiguration : ISdkConfiguration
    {
        /// <summary>
        /// Gets an username used when establishing connection to the AMQP broker
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets a password used when establishing connection to the AMQP broker
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets a value specifying the host name of the AMQP broker
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets the port number used to connect to the AMQP broker
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Gets a value specifying the virtual host name of the AMQP broker
        /// </summary>
        public string VirtualHost { get; }

        /// <summary>
        /// Gets a value specifying whether the connection to AMQP broker should use SSL encryption
        /// </summary>
        public bool UseSsl { get; }

        /// <summary>
        /// Gets the server name that will be used to check against SSL certificate
        /// </summary>
        public string SslServerName { get; }

        /// <summary>
        /// Gets a node id
        /// </summary>
        public int NodeId { get; }

        /// <summary>
        /// Gets the BookmakerId associated with the current configuration or 0 if none is provided
        /// </summary>
        public int BookmakerId { get; }

        /// <summary>
        /// Gets the channel identifier associated with the current configuration or 0 if none is provided
        /// </summary>
        public int LimitId { get; }

        /// <summary>
        /// Gets the default currency associated with the current configuration or a null reference if none is provided.
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Gets the <see cref="SenderChannel" /> specifying the associated channel or a null reference if none is specified.
        /// </summary>
        public SenderChannel? Channel { get; }

        /// <summary>
        /// Gets a value indication whether statistics collection is enabled
        /// </summary>
        public bool StatisticsEnabled { get; }

        /// <summary>
        /// Gets the timeout for automatically collecting statistics (in sec)
        /// </summary>
        public int StatisticsTimeout { get; }

        /// <summary>
        /// Gets the limit of records for automatically writing statistics (in number of records)
        /// </summary>
        public int StatisticsRecordLimit { get; }

        /// <summary>
        /// Gets the access token for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        /// <value>The access token</value>
        public string AccessToken { get; }

        /// <summary>
        /// Gets the UF environment for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        /// <value>The UF environment</value>
        public UfEnvironment? UfEnvironment { get; }

        /// <summary>
        /// Gets a value indicating whether additional market specifiers should be added
        /// </summary>
        /// <value><c>true</c> if [provide additional market specifiers]; otherwise, <c>false</c></value>
        public bool ProvideAdditionalMarketSpecifiers { get; }

        /// <summary>
        /// Should the rabbit consumer channel be exclusive
        /// </summary>
        public bool ExclusiveConsumer { get; }

        /// <summary>
        /// Gets the Keycloak host for authorization
        /// </summary>
        public string KeycloakHost { get; }

        /// <summary>
        /// Gets the username used to connect authenticate to Keycloak
        /// </summary>
        public string KeycloakUsername { get; }

        /// <summary>
        /// Gets the password used to connect authenticate to Keycloak
        /// </summary>
        public string KeycloakPassword { get; }

        /// <summary>
        /// Gets the secret used to connect authenticate to Keycloak
        /// </summary>
        public string KeycloakSecret { get; }

        /// <summary>
        /// Gets the Client API host
        /// </summary>
        public string MtsClientApiHost { get; }

        /// <summary>
        /// Gets the ticket response timeout(ms) for tickets using "live" selectionId
        /// </summary>
        public int TicketResponseTimeoutLive { get; }

        /// <summary>
        /// Gets the ticket response timeout(ms) for tickets using "prematch" selectionId
        /// </summary>
        /// <value>Gets the ticket response timeout(ms) for tickets using "prematch" selectionId</value>
        public int TicketResponseTimeoutPrematch { get; }

        /// <summary>
        /// Gets the ticket cancellation response timeout(ms)
        /// </summary>
        public int TicketCancellationResponseTimeout { get; }

        /// <summary>
        /// Gets the ticket cashout response timeout(ms)
        /// </summary>
        public int TicketCashoutResponseTimeout { get; }

        /// <summary>
        /// Gets the ticket non-sportradar settle response timeout(ms)
        /// </summary>
        public int TicketNonSrSettleResponseTimeout { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkConfiguration"/> class
        /// </summary>
        /// <param name="username">The username used when connecting to AMQP broker</param>
        /// <param name="password">The password used when connecting to AMQP broker</param>
        /// <param name="host">The host name of the AMQP broker</param>
        /// <param name="vhost">The virtual host defined on the AMQP broker</param>
        /// <param name="useSsl">Value indicating whether SSL should be used when connecting to AMQP</param>
        /// <param name="sslServerName">The server name that will be used to check against SSL certificate</param>
        /// <param name="nodeId"> The value uniquely identifying the SDK instance associated with the current configuration</param>
        /// <param name="bookmakerId">The bookmaker id assigned to the customer by the MTS CI</param>
        /// <param name="limitId">The value specifying the limits of the placed tickets</param>
        /// <param name="currency">The currency of the placed tickets or a null reference</param>
        /// <param name="channel">The <see cref="SenderChannel"/> specifying the origin of the tickets or a null reference</param>
        /// <param name="accessToken">The access token for the UF feed (only necessary if UF selections will be build)</param>
        /// <param name="ufEnvironment">The UF environment for the UF feed (only necessary if UF selections will be build)</param>
        /// <param name="provideAdditionalMarketSpecifiers">The value indicating if the additional market specifiers should be provided</param>
        /// <param name="port">The port number used to connect to the AMQP broker</param>
        /// <param name="exclusiveConsumer">Should the consumer channel be exclusive</param>
        /// <param name="keycloakHost">The Keycloak host for authorization</param>
        /// <param name="keycloakUsername">The username used to connect authenticate to Keycloak</param>
        /// <param name="keycloakPassword">The password used to connect authenticate to Keycloak</param>
        /// <param name="keycloakSecret">The secret used to connect authenticate to Keycloak</param>
        /// <param name="mtsClientApiHost">The Client API host</param>
        /// <param name="ticketResponseTimeoutLive">The ticket response timeout(ms) for tickets using "live" selectionId</param>
        /// <param name="ticketResponseTimeoutPrematch">The ticket response timeout(ms) for tickets using "prematch" selectionId</param>
        /// <param name="ticketCancellationResponseTimeout">The ticket cancellation response timeout(ms)</param>
        /// <param name="ticketCashoutResponseTimeout">The ticket cashout response timeout(ms)</param>
        /// <param name="ticketNonSrSettleResponseTimeout">The ticket cashout response timeout(ms)</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        public SdkConfiguration(
            string username,
            string password,
            string host,
            string vhost = null,
            bool useSsl = true,
            string sslServerName = null,
            int nodeId = 1,
            int bookmakerId = 0,
            int limitId = 0,
            string currency = null,
            SenderChannel? channel = null,
            string accessToken = null,
            UfEnvironment? ufEnvironment = null,
            bool provideAdditionalMarketSpecifiers = true,
            int port = 0,
            bool exclusiveConsumer = true,
            string keycloakHost = null,
            string keycloakUsername = null,
            string keycloakPassword = null,
            string keycloakSecret = null,
            string mtsClientApiHost = null,
            int ticketResponseTimeoutLive = SdkInfo.TicketResponseTimeoutLiveDefault,
            int ticketResponseTimeoutPrematch = SdkInfo.TicketResponseTimeoutPrematchDefault,
            int ticketCancellationResponseTimeout = SdkInfo.TicketCancellationResponseTimeoutDefault,
            int ticketCashoutResponseTimeout = SdkInfo.TicketCashoutResponseTimeoutDefault,
            int ticketNonSrSettleResponseTimeout = SdkInfo.TicketCashoutResponseTimeoutDefault)
        {
            Guard.Argument(username, nameof(username)).NotNull().NotEmpty();
            Guard.Argument(password, nameof(password)).NotNull().NotEmpty();
            Guard.Argument(host, nameof(host)).NotNull().NotEmpty();

            CheckParameters(ticketResponseTimeoutLive, ticketResponseTimeoutPrematch, ticketCancellationResponseTimeout, ticketCashoutResponseTimeout, ticketNonSrSettleResponseTimeout);

            Username = username;
            Password = password;
            Host = host;
            VirtualHost = string.IsNullOrEmpty(vhost) ? "/" + Username : vhost;
            if (!VirtualHost.StartsWith("/"))
            {
                VirtualHost = "/" + VirtualHost;
            }
            UseSsl = useSsl;
            SslServerName = sslServerName;
            NodeId = nodeId > 0 ? nodeId : 1;
            BookmakerId = bookmakerId;
            LimitId = limitId;
            Currency = currency;
            Channel = channel;
            StatisticsEnabled = false;
            StatisticsRecordLimit = 1000000;
            StatisticsTimeout = 3600;

            AccessToken = accessToken ?? string.Empty;
            UfEnvironment = ufEnvironment;
            ProvideAdditionalMarketSpecifiers = provideAdditionalMarketSpecifiers;

            Port = UseSsl ? 5671 : 5672;
            if (port > 0)
            {
                Port = port;
            }
            ExclusiveConsumer = exclusiveConsumer;

            if (Host.Contains(":"))
            {
                throw new ArgumentException("Host can not contain port number. Only domain name or ip address. E.g. mtsgate-ci.betradar.com");
            }

            KeycloakHost = keycloakHost;
            KeycloakUsername = keycloakUsername;
            KeycloakPassword = keycloakPassword;
            KeycloakSecret = keycloakSecret;
            MtsClientApiHost = mtsClientApiHost;

            if (MtsClientApiHost != null)
            {
                if (KeycloakHost == null)
                {
                    throw new ArgumentException("KeycloakHost must be set.");
                }

                if (KeycloakSecret == null)
                {
                    throw new ArgumentException("KeycloakSecret must be set.");
                }
            }

            TicketResponseTimeoutLive = ticketResponseTimeoutLive;
            TicketResponseTimeoutPrematch = ticketResponseTimeoutPrematch;
            TicketCancellationResponseTimeout = ticketCancellationResponseTimeout;
            TicketCashoutResponseTimeout = ticketCashoutResponseTimeout;
            TicketNonSrSettleResponseTimeout = ticketNonSrSettleResponseTimeout;

            ObjectInvariant();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkConfiguration"/> class
        /// </summary>
        /// <param name="section">A <see cref="SdkConfigurationSection"/> instance containing config values</param>
        public SdkConfiguration(ISdkConfigurationSection section)
        {
            Guard.Argument(section, nameof(section)).NotNull();

            Username = section.Username;
            Password = section.Password;
            Host = section.Host;
            VirtualHost = string.IsNullOrEmpty(section.VirtualHost) ? "/" + Username : section.VirtualHost;
            if (!VirtualHost.StartsWith("/"))
            {
                VirtualHost = "/" + VirtualHost;
            }
            UseSsl = section.UseSsl;
            SslServerName = section.SslServerName;
            NodeId = section.NodeId;
            BookmakerId = section.BookmakerId;
            LimitId = section.LimitId;
            Currency = section.Currency;
            Channel = section.Channel;

            StatisticsEnabled = section.StatisticsEnabled;
            StatisticsRecordLimit = section.StatisticsRecordLimit;
            StatisticsTimeout = section.StatisticsTimeout;
            AccessToken = section.AccessToken;
            UfEnvironment = section.UfEnvironment;
            ProvideAdditionalMarketSpecifiers = section.ProvideAdditionalMarketSpecifiers;
            Port = UseSsl ? 5671 : 5672;
            if (section.Port > 0)
            {
                Port = section.Port;
            }
            ExclusiveConsumer = section.ExclusiveConsumer;

            if (Host.Contains(":"))
            {
                throw new ArgumentException("Host can not contain port number. Only domain name or ip address. E.g. mtsgate-ci.betradar.com");
            }

            KeycloakHost = section.KeycloakHost;
            KeycloakUsername = section.KeycloakUsername;
            KeycloakPassword = section.KeycloakPassword;
            KeycloakSecret = section.KeycloakSecret;
            MtsClientApiHost = section.MtsClientApiHost;

            if (MtsClientApiHost != null)
            {
                if (KeycloakHost == null)
                {
                    throw new ArgumentException("KeycloakHost must be set.");
                }
                if (KeycloakSecret == null)
                {
                    throw new ArgumentException("KeycloakSecret must be set.");
                }
            }

            TicketResponseTimeoutLive = section.TicketResponseTimeoutLive;
            TicketResponseTimeoutPrematch = section.TicketResponseTimeoutPrematch;
            TicketCancellationResponseTimeout = section.TicketCancellationResponseTimeout;
            TicketCashoutResponseTimeout = section.TicketCashoutResponseTimeout;
            TicketNonSrSettleResponseTimeout = section.TicketNonSrSettleResponseTimeout;

            ObjectInvariant();
        }

        private void CheckParameters(int ticketResponseTimeoutLive,
                                     int ticketResponseTimeoutPrematch,
                                     int ticketCancellationResponseTimeout,
                                     int ticketCashoutResponseTimeout,
                                     int ticketNonSrSettleResponseTimeout)
        {
            if (ticketResponseTimeoutLive < SdkInfo.TicketResponseTimeoutLiveMin)
            {
                throw new ArgumentException($"TicketResponseTimeoutLive must be more than {SdkInfo.TicketResponseTimeoutLiveMin}ms");
            }
            if (ticketResponseTimeoutLive > SdkInfo.TicketResponseTimeoutLiveMax)
            {
                throw new ArgumentException($"TicketResponseTimeoutLive must be less than {SdkInfo.TicketResponseTimeoutLiveMax}ms");
            }
            if (ticketResponseTimeoutPrematch < SdkInfo.TicketResponseTimeoutPrematchMin)
            {
                throw new ArgumentException($"TicketResponseTimeoutPrematch must be more than {SdkInfo.TicketResponseTimeoutPrematchMin}ms");
            }
            if (ticketResponseTimeoutPrematch > SdkInfo.TicketResponseTimeoutPrematchMax)
            {
                throw new ArgumentException($"TicketResponseTimeoutPrematch must be less than {SdkInfo.TicketResponseTimeoutPrematchMax}ms");
            }
            if (ticketCancellationResponseTimeout < SdkInfo.TicketCancellationResponseTimeoutMin)
            {
                throw new ArgumentException($"TicketCancellationResponseTimeout must be more than {SdkInfo.TicketCancellationResponseTimeoutMin}ms");
            }
            if (ticketCancellationResponseTimeout > SdkInfo.TicketCancellationResponseTimeoutMax)
            {
                throw new ArgumentException($"TicketCancellationResponseTimeout must be less than {SdkInfo.TicketCancellationResponseTimeoutMax}ms");
            }
            if (ticketCashoutResponseTimeout < SdkInfo.TicketCashoutResponseTimeoutMin)
            {
                throw new ArgumentException($"TicketCashoutResponseTimeout must be more than {SdkInfo.TicketCashoutResponseTimeoutMin}ms");
            }
            if (ticketCashoutResponseTimeout > SdkInfo.TicketCashoutResponseTimeoutMax)
            {
                throw new ArgumentException($"TicketCashoutResponseTimeout must be less than {SdkInfo.TicketCashoutResponseTimeoutMax}ms");
            }
            if (ticketNonSrSettleResponseTimeout < SdkInfo.TicketNonSrResponseTimeoutMin)
            {
                throw new ArgumentException($"TicketNonSrResponseTimeout must be more than {SdkInfo.TicketNonSrResponseTimeoutMin}ms");
            }
            if (ticketNonSrSettleResponseTimeout > SdkInfo.TicketNonSrResponseTimeoutMax)
            {
                throw new ArgumentException($"TicketNonSrResponseTimeout must be less than {SdkInfo.TicketNonSrResponseTimeoutMax}ms");
            }
        }

        /// <summary>
        /// Defined field invariants needed by code contracts
        /// </summary>
        private void ObjectInvariant()
        {
            Guard.Argument(Username, nameof(Username)).NotNull().NotEmpty();
            Guard.Argument(Password, nameof(Password)).NotNull().NotEmpty();
            Guard.Argument(Host, nameof(Host)).NotNull().NotEmpty();
            Guard.Argument(VirtualHost, nameof(VirtualHost)).NotNull().NotEmpty();
            Guard.Argument(Port, nameof(Port)).Positive();
            Guard.Argument(NodeId, nameof(NodeId)).Positive();
            Guard.Argument(BookmakerId, nameof(BookmakerId)).NotNegative();
            Guard.Argument(LimitId, nameof(LimitId)).NotNegative();
            Guard.Argument(Currency, nameof(Currency)).Require(Currency == null || (Currency.Length >= 3 && Currency.Length <= 4));
            Guard.Argument(Host, nameof(Host)).Require(!Host.Contains(":"), s => "Host can not contain port number. Only domain name or ip address. E.g. mtsgate-ci.betradar.com");
            Guard.Argument(TicketResponseTimeoutLive, nameof(TicketResponseTimeoutLive)).Require(TicketResponseTimeoutLive >= SdkInfo.TicketResponseTimeoutLiveMin, s => $"TicketResponseTimeoutLive must be more than {SdkInfo.TicketResponseTimeoutLiveMin}ms");
            Guard.Argument(TicketResponseTimeoutLive, nameof(TicketResponseTimeoutLive)).Require(TicketResponseTimeoutLive <= SdkInfo.TicketResponseTimeoutLiveMax, s => $"TicketResponseTimeoutLive must be less than {SdkInfo.TicketResponseTimeoutLiveMax}ms");
            Guard.Argument(TicketResponseTimeoutPrematch, nameof(TicketResponseTimeoutPrematch)).Require(TicketResponseTimeoutPrematch >= SdkInfo.TicketResponseTimeoutPrematchMin, s => $"TicketResponseTimeoutPrematch must be more than {SdkInfo.TicketResponseTimeoutPrematchMin}ms");
            Guard.Argument(TicketResponseTimeoutPrematch, nameof(TicketResponseTimeoutPrematch)).Require(TicketResponseTimeoutPrematch <= SdkInfo.TicketResponseTimeoutPrematchMax, s => $"TicketResponseTimeoutPrematch must be less than {SdkInfo.TicketResponseTimeoutPrematchMax}ms");
            Guard.Argument(TicketCancellationResponseTimeout, nameof(TicketCancellationResponseTimeout)).Require(TicketCancellationResponseTimeout >= SdkInfo.TicketCancellationResponseTimeoutMin, s => $"TicketCancellationResponseTimeout must be more than {SdkInfo.TicketCancellationResponseTimeoutMin}ms");
            Guard.Argument(TicketCancellationResponseTimeout, nameof(TicketCancellationResponseTimeout)).Require(TicketCancellationResponseTimeout <= SdkInfo.TicketCancellationResponseTimeoutMax, s => $"TicketCancellationResponseTimeout must be less than {SdkInfo.TicketCancellationResponseTimeoutMax}ms");
            Guard.Argument(TicketCashoutResponseTimeout, nameof(TicketCashoutResponseTimeout)).Require(TicketCashoutResponseTimeout >= SdkInfo.TicketCashoutResponseTimeoutMin, s => $"TicketCashoutResponseTimeout must be more than {SdkInfo.TicketCashoutResponseTimeoutMin}ms");
            Guard.Argument(TicketCashoutResponseTimeout, nameof(TicketCashoutResponseTimeout)).Require(TicketCashoutResponseTimeout <= SdkInfo.TicketCashoutResponseTimeoutMax, s => $"TicketCashoutResponseTimeout must be less than {SdkInfo.TicketCashoutResponseTimeoutMax}ms");
            Guard.Argument(TicketNonSrSettleResponseTimeout, nameof(TicketNonSrSettleResponseTimeout)).Require(TicketNonSrSettleResponseTimeout >= SdkInfo.TicketNonSrResponseTimeoutMin, s => $"TicketNonSrSettleResponseTimeout must be more than {SdkInfo.TicketNonSrResponseTimeoutMin}ms");
            Guard.Argument(TicketNonSrSettleResponseTimeout, nameof(TicketNonSrSettleResponseTimeout)).Require(TicketNonSrSettleResponseTimeout <= SdkInfo.TicketNonSrResponseTimeoutMax, s => $"TicketNonSrSettleResponseTimeout must be less than {SdkInfo.TicketNonSrResponseTimeoutMax}ms");
        }
    }
}