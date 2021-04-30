/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities
{
    /// <summary>
    /// Defines a contract implemented by classes representing configuration
    /// </summary>
    public interface ISdkConfiguration
    {
        /// <summary>
        /// Gets an username used when establishing connection to the AMQP broker
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets a password used when establishing connection to the AMQP broker
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets a value specifying the host name of the AMQP broker
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Gets the port used to connect to AMQP broker
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets a value specifying the virtual host name of the AMQP broker
        /// </summary>
        string VirtualHost { get; }

        /// <summary>
        /// Gets a value specifying whether the connection to AMQP broker should use SSL encryption
        /// </summary>
        bool UseSsl { get; }

        /// <summary>
        /// Gets the server name that will be used to check against SSL certificate
        /// </summary>
        string SslServerName { get; }

        /// <summary>
        /// Gets a nodeId
        /// </summary>
        int NodeId { get; }

        /// <summary>
        /// Gets the BookmakerId associated with the current configuration or 0 if none is provided
        /// </summary>
        int BookmakerId { get; }

        /// <summary>
        /// Gets the channel identifier associated with the current configuration or 0 if none is provided
        /// </summary>
        int LimitId { get; }

        /// <summary>
        /// Gets the default currency associated with the current configuration or a null reference if none is provided
        /// </summary>
        string Currency { get; }

        /// <summary>
        /// Gets the <see cref="SenderChannel"/> specifying the associated channel or a null reference if none is specified
        /// </summary>
        SenderChannel? Channel { get; }

        /// <summary>
        /// Gets the access token for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Gets the UF environment for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        /// <value>The UF environment</value>
        UfEnvironment? UfEnvironment { get; }

        /// <summary>
        /// Gets a value indicating whether additional market specifiers should be added
        /// </summary>
        /// <value><c>true</c> if [provide additional market specifiers]; otherwise, <c>false</c></value>
        /// <remarks>If this is set to true and the user uses UOF markets, when there are special cases (market 215, or $score in SOV/SBV template), sdk automatically tries to add appropriate specifier; if set to false, user will need to add this manually</remarks>
        bool ProvideAdditionalMarketSpecifiers { get; }

        /// <summary>
        /// Gets a value indication whether statistics collection is enabled
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
        /// Gets the ticket response timeout(ms) for tickets using "prematch" selectionId
        /// </summary>
        /// <value>Gets the ticket response timeout(ms) for tickets using "prematch" selectionId</value>
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
        int TicketNonSrSettleResponseTimeout { get;}
    }
}