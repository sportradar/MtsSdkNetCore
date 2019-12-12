/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.API
{
    /// <summary>
    /// Defines a contract for classes implementing builder for <see cref="ISdkConfiguration"/>
    /// </summary>
    public interface ISdkConfigurationBuilder
    {
        /// <summary>
        /// Sets the username
        /// </summary>
        /// <param name="username">The username to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetUsername(string username);

        /// <summary>
        /// Sets the password
        /// </summary>
        /// <param name="password">The password to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetPassword(string password);

        /// <summary>
        /// Sets the host used to connect to AMQP broker
        /// </summary>
        /// <param name="host">The host to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetHost(string host);

        /// <summary>
        /// Sets the port used to connect to AMQP broker
        /// </summary>
        /// <param name="port">The port to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        /// <remarks>Port should be chosen through the UseSsl property. Manually setting port number should be used only when non-default port is required</remarks>
        ISdkConfigurationBuilder SetPort(int port);

        /// <summary>
        /// Sets the vhost (format: '/vhost')
        /// </summary>
        /// <param name="vhost">The vhost to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetVirtualHost(string vhost);

        /// <summary>
        /// Sets the node id
        /// </summary>
        /// <param name="nodeId">The node id to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetNode(int nodeId);

        /// <summary>
        /// Sets the value indicating whether a secure connection to the message broker should be used
        /// </summary>
        /// <param name="useSsl">The value to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetUseSsl(bool useSsl);

        /// <summary>
        /// Sets the bookmakerId
        /// </summary>
        /// <param name="bookmakerId">The bookmakerId to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetBookmakerId(int bookmakerId);

        /// <summary>
        /// Sets the limitId
        /// </summary>
        /// <param name="limitId">The limitId to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetLimitId(int limitId);

        /// <summary>
        /// Sets the currency
        /// </summary>
        /// <param name="currency">The currency to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetCurrency(string currency);

        /// <summary>
        /// Sets the sender channel
        /// </summary>
        /// <param name="channel">The channel to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetSenderChannel(SenderChannel channel);

        /// <summary>
        /// Sets the access token
        /// </summary>
        /// <param name="accessToken">The accessToken to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetAccessToken(string accessToken);

        /// <summary>
        /// Sets the uf environment
        /// </summary>
        /// <param name="ufEnvironment">The uf environment to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetUfEnvironment(UfEnvironment ufEnvironment);

        /// <summary>
        /// This value is used to indicate if the sdk should add market specifiers for specific markets. Only used when building selection using UnifiedOdds ids. (default: true)
        /// </summary>
        /// <param name="provideAdditionalMarketSpecifiers">The value to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        /// <remarks>If this is set to true and the user uses UOF markets, when there are special cases (market 215, or $score in SOV/SBV template), sdk automatically tries to add appropriate specifier; if set to false, user will need to add this manually</remarks>
        ISdkConfigurationBuilder SetProvideAdditionalMarketSpecifiers(bool provideAdditionalMarketSpecifiers);

        /// <summary>
        /// Sets the value indicating whether the rabbit consumer channel should be exclusive
        /// </summary>
        /// <param name="exclusiveConsumer">The value to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetExclusiveConsumer(bool exclusiveConsumer);

        /// <summary>
        /// Sets the Keycloak host for authorization
        /// </summary>
        /// <param name="keycloakHost">The Keycloak host to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetKeycloakHost(string keycloakHost);

        /// <summary>
        /// Sets the username used to connect authenticate to Keycloak
        /// </summary>
        /// <param name="keycloakUsername">The username to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetKeycloakUsername(string keycloakUsername);

        /// <summary>
        /// Sets the password used to connect authenticate to Keycloak
        /// </summary>
        /// <param name="keycloakPassword">The password to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetKeycloakPassword(string keycloakPassword);

        /// <summary>
        /// Sets the secret used to connect authenticate to Keycloak
        /// </summary>
        /// <param name="keycloakSecret">The secret to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetKeycloakSecret(string keycloakSecret);

        /// <summary>
        /// Sets the Client API host
        /// </summary>
        /// <param name="mtsClientApiHost">The Client API host to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetMtsClientApiHost(string mtsClientApiHost);

        /// <summary>
        /// Sets the ticket response timeout(ms) (sets both live and prematch timeouts)
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetTicketResponseTimeout(int responseTimeout);

        /// <summary>
        /// Sets the ticket response timeout(ms) for tickets using "live" selectionId
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetTicketResponseTimeoutLive(int responseTimeout);

        /// <summary>
        /// Sets the ticket response timeout(ms) for tickets using "prematch" selectionId
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetTicketResponseTimeoutPrematch(int responseTimeout);

        /// <summary>
        /// Sets the ticket cancellation response timeout(ms)
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetTicketCancellationResponseTimeout(int responseTimeout);

        /// <summary>
        /// Sets the ticket cashout response timeout(ms)
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetTicketCashoutResponseTimeout(int responseTimeout);

        /// <summary>
        /// Sets the ticket non-sr settle response timeout(ms)
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        ISdkConfigurationBuilder SetNonSrSettleResponseTimeout(int responseTimeout);

        /// <summary>
        /// Builds the <see cref="ISdkConfiguration" />
        /// </summary>
        /// <returns>Returns a <see cref="ISdkConfiguration"/></returns>
        ISdkConfiguration Build();
    }
}
