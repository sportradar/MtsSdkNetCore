/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Defines a contract for classes implementing sdk configuration section in the app.config
    /// </summary>
    internal interface ISdkConfigurationSection
    {
        /// <summary>
        /// Gets the username used to connect to MTS
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the password used to connect to MTS
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the hostname of the broker
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Gets the port number used to connect to the broker
        /// </summary>
        /// <remarks>Port should be chosen through the useSsl property. Manually setting port number should be used only when non-default port is required</remarks>
        int Port { get; }

        /// <summary>
        /// Gets the virtual host name
        /// </summary>
        string VirtualHost { get; }

        /// <summary>
        /// Gets the node id for this instance of sdk
        /// </summary>
        int NodeId { get; }

        /// <summary>
        /// Gets a value specifying whether the connection to AMQP broker should use SSL encryption
        /// </summary>
        bool UseSsl { get; }

        /// <summary>
        /// Gets the server name that will be used to check against SSL certificate
        /// </summary>
        string SslServerName { get; }

        /// <summary>
        /// Gets the default sender bookmakerId
        /// </summary>
        int BookmakerId { get; }

        /// <summary>
        /// Gets the default sender limitId
        /// </summary>
        int LimitId { get; }

        /// <summary>
        /// Gets the default sender currency sign (3 or 4 letter)
        /// </summary>
        string Currency { get; }

        /// <summary>
        /// Gets the default sender channel (see <see cref="SenderChannel"/> for possible values)
        /// </summary>
        SenderChannel? Channel { get; }

        /// <summary>
        /// Gets the access token for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Gets the uf environment for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        UfEnvironment? UfEnvironment { get; }

        /// <summary>
        /// This value is used to indicate if the sdk should add market specifiers for specific markets. Only used when building selection using UnifiedOdds ids. (default: true)
        /// </summary>
        /// <remarks>If this is set to true and the user uses UOF markets, when there are special cases (market 215, or $score in SOV/SBV template), sdk automatically tries to add appropriate specifier; if set to false, user will need to add this manually</remarks>
        bool ProvideAdditionalMarketSpecifiers { get; }

        /// <summary>
        /// Is statistics collecting enabled
        /// </summary>
        bool StatisticsEnabled { get; }

        /// <summary>
        /// Gets the timeout for automatically collecting statistics
        /// </summary>
        int StatisticsTimeout { get; }

        /// <summary>
        /// Gets the limit of records for automatically writing statistics
        /// </summary>
        int StatisticsRecordLimit { get; }

        /// <summary>
        /// Gets the file path to the configuration file for the log4net repository used by the SDK
        /// </summary>
        string SdkLogConfigPath { get; }

        /// <summary>
        /// Should the rabbit consumer channel be exclusive
        /// </summary>
        bool ExclusiveConsumer { get; }

        /// <summary>
        /// Gets the Keycloak host for authorization
        /// </summary>
        string KeycloakHost { get; }

        /// <summary>
        /// Gets the username used to connect authenticate to Keycloak
        /// </summary>
        string KeycloakUsername { get; }

        /// <summary>
        /// Gets the password used to connect authenticate to Keycloak
        /// </summary>
        string KeycloakPassword { get; }

        /// <summary>
        /// Gets the secret used to connect authenticate to Keycloak
        /// </summary>
        string KeycloakSecret { get; }

        /// <summary>
        /// Gets the Client API host
        /// </summary>
        string MtsClientApiHost { get; }

        /// <summary>
        /// Gets the ticket response timeout(ms) for tickets using "live" selectionId
        /// </summary>
        int TicketResponseTimeoutLive { get; }

        /// <summary>
        /// Gets the ticket response timeout(ms) for tickets using "live" selectionId
        /// </summary>
        int TicketResponseTimeoutPrematch { get; }

        /// <summary>
        /// Gets the ticket cancellation response timeout(ms)
        /// </summary>
        int TicketCancellationResponseTimeout { get; }

        /// <summary>
        /// Gets the ticket cashout response timeout(ms)
        /// </summary>
        int TicketCashoutResponseTimeout { get; }

        /// <summary>
        /// Gets the ticket non-sportradar settle response timeout(ms)
        /// </summary>
        int TicketNonSrSettleResponseTimeout { get; }
    }
}