/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Configuration;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Enums;
using ConfigurationSection = System.Configuration.ConfigurationSection;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Represents the SDK settings read from app.config file
    /// </summary>
    internal class SdkConfigurationSection : ConfigurationSection, ISdkConfigurationSection
    {
        /// <summary>
        /// The name of the section element in the app.config file
        /// </summary>
        private const string SectionName = "mtsSdkSection";

        /// <summary>
        /// Gets the username used to connect to MTS
        /// </summary>
        [ConfigurationProperty("username", IsRequired = true)]
        public string Username => (string)base["username"];

        /// <summary>
        /// Gets the password used to connect to MTS
        /// </summary>
        [ConfigurationProperty("password", IsRequired = true)]
        public string Password => (string)base["password"];

        /// <summary>
        /// Gets the hostname of the broker
        /// </summary>
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host => (string)base["host"];

        /// <summary>
        /// Gets the port number used to connect to the broker
        /// </summary>
        /// <remarks>Port should be chosen through the useSsl property. Manually setting port number should be used only when non-default port is required</remarks>
        [ConfigurationProperty("port", IsRequired = false, DefaultValue = 0)]
        public int Port => (int)base["port"];
         
        /// <summary>
        /// Gets the virtual host name
        /// </summary>
        [ConfigurationProperty("vhost", IsRequired = false)]
        public string VirtualHost => (string)base["vhost"];

        /// <summary>
        /// Gets the node id for this instance of sdk
        /// </summary>
        [ConfigurationProperty("node", IsRequired = false, DefaultValue = 1)]
        public int NodeId => (int)base["node"];

        /// <summary>
        /// Gets a value specifying whether the connection to AMQP broker should use SSL encryption
        /// </summary>
        [ConfigurationProperty("useSsl", IsRequired = false, DefaultValue = true)]
        public bool UseSsl => (bool)base["useSsl"];

        /// <summary>
        /// Gets the server name that will be used to check against SSL certificate
        /// </summary>
        [ConfigurationProperty("sslServerName", IsRequired = false)]
        public string SslServerName => GetNullableString("sslServerName");

        /// <summary>
        /// Gets the default sender bookmakerId
        /// </summary>
        [ConfigurationProperty("bookmakerId", IsRequired = false, DefaultValue = 0)]
        public int BookmakerId => (int)base["bookmakerId"];

        /// <summary>
        /// Gets the default sender limitId
        /// </summary>
        [ConfigurationProperty("limitId", IsRequired = false, DefaultValue = 0)]
        public int LimitId => (int)base["limitId"];

        /// <summary>
        /// Gets the default sender currency sign (3 or 4 letter)
        /// </summary>
        [ConfigurationProperty("currency", IsRequired = false)]
        public string Currency => GetNullableString("currency");

        /// <summary>
        /// Gets the default sender channel (see <see cref="SenderChannel"/> for possible values)
        /// </summary>
        [ConfigurationProperty("channel", IsRequired = false, DefaultValue = null)]
        public SenderChannel? Channel => (SenderChannel?)base["channel"];

        /// <summary>
        /// Gets the access token for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        [ConfigurationProperty("accessToken", IsRequired = false)]
        public string AccessToken => (string)base["accessToken"];

        /// <summary>
        /// Gets the uf environment for the UF feed (only necessary if UF selections will be build)
        /// </summary>
        [ConfigurationProperty("ufEnvironment", IsRequired = false)]
        public UfEnvironment? UfEnvironment => (UfEnvironment?)base["ufEnvironment"];

        /// <summary>
        /// This value is used to indicate if the sdk should add market specifiers for specific markets. Only used when building selection using UnifiedOdds ids. (default: true)
        /// </summary>
        /// <remarks>If this is set to true and the user uses UOF markets, when there are special cases (market 215, or $score in SOV/SBV template), sdk automatically tries to add appropriate specifier; if set to false, user will need to add this manually</remarks>
        [ConfigurationProperty("provideAdditionalMarketSpecifiers", IsRequired = false, DefaultValue = true)]
        public bool ProvideAdditionalMarketSpecifiers => (bool)base["provideAdditionalMarketSpecifiers"];

        /// <summary>
        /// Should the rabbit consumer channel be exclusive
        /// </summary>
        [ConfigurationProperty("exclusiveConsumer", IsRequired = false, DefaultValue = true)]
        public bool ExclusiveConsumer => (bool)base["exclusiveConsumer"];

        // below are private settings, not (yet) exposed to users
        /// <summary>
        /// Is statistics collecting enabled
        /// </summary>
        [ConfigurationProperty("statsEnabled", IsRequired = false, DefaultValue = false)]
        public bool StatisticsEnabled => (bool)base["statsEnabled"];

        /// <summary>
        /// Gets the timeout for automatically collecting statistics
        /// </summary>
        [ConfigurationProperty("statsTimeout", IsRequired = false, DefaultValue = 3600)]
        public int StatisticsTimeout => (int)base["statsTimeout"];

        /// <summary>
        /// Gets the limit of records for automatically writing statistics
        /// </summary>
        [ConfigurationProperty("statsMaxRecord", IsRequired = false, DefaultValue = 1000000)]
        public int StatisticsRecordLimit => (int)base["statsMaxRecord"];

        /// <summary>
        /// Gets the Keycloak host for authorization
        /// </summary>
        [ConfigurationProperty("keycloakHost", IsRequired = false)]
        public string KeycloakHost => GetNullableString("keycloakHost");

        /// <summary>
        /// Gets the username used to connect authenticate to Keycloak
        /// </summary>
        [ConfigurationProperty("keycloakUsername", IsRequired = false)]
        public string KeycloakUsername => GetNullableString("keycloakUsername");

        /// <summary>
        /// Gets the password used to connect authenticate to Keycloak
        /// </summary>
        [ConfigurationProperty("keycloakPassword", IsRequired = false)]
        public string KeycloakPassword => GetNullableString("keycloakPassword");

        /// <summary>
        /// Gets the secret used to connect authenticate to Keycloak
        /// </summary>
        [ConfigurationProperty("keycloakSecret", IsRequired = false)]
        public string KeycloakSecret => GetNullableString("keycloakSecret");

        /// <summary>
        /// Gets the Client API host
        /// </summary>
        [ConfigurationProperty("mtsClientApiHost", IsRequired = false)]
        public string MtsClientApiHost => GetNullableString("mtsClientApiHost");

        /// <summary>
        /// Gets the ticket response timeout (ms)
        /// </summary>
        [ConfigurationProperty("ticketResponseTimeout", IsRequired = false, DefaultValue = SdkInfo.TicketResponseTimeoutLiveDefault)]
        public int TicketResponseTimeoutLive => (int)base["ticketResponseTimeout"];

        /// <summary>
        /// Gets the ticket response timeout (ms)
        /// </summary>
        [ConfigurationProperty("ticketResponseTimeoutPrematch", IsRequired = false, DefaultValue = SdkInfo.TicketResponseTimeoutPrematchDefault)]
        public int TicketResponseTimeoutPrematch => (int)base["ticketResponseTimeoutPrematch"];

        /// <summary>
        /// Gets the ticket cancellation response timeout (ms)
        /// </summary>
        [ConfigurationProperty("ticketCancellationResponseTimeout", IsRequired = false, DefaultValue = SdkInfo.TicketCancellationResponseTimeoutDefault)]
        public int TicketCancellationResponseTimeout => (int)base["ticketCancellationResponseTimeout"];

        /// <summary>
        /// Gets the ticket cashout response timeout (ms)
        /// </summary>
        [ConfigurationProperty("ticketCashoutResponseTimeout", IsRequired = false, DefaultValue = SdkInfo.TicketCashoutResponseTimeoutDefault)]
        public int TicketCashoutResponseTimeout => (int)base["ticketCashoutResponseTimeout"];

        /// <summary>
        /// Gets the non-sr ticket settlement response timeout (ms)
        /// </summary>
        [ConfigurationProperty("ticketNonSrSettleResponseTimeout", IsRequired = false, DefaultValue = SdkInfo.TicketNonSrResponseTimeoutDefault)]
        public int TicketNonSrSettleResponseTimeout => (int)base["ticketNonSrSettleResponseTimeout"];

        /// <summary>
        /// Gets the file path to the configuration file for the log4net repository used by the SDK
        /// </summary>
        [ConfigurationProperty("sdkLogConfigPath", IsRequired = false)]
        public string SdkLogConfigPath => (string)base["sdkLogConfigPath"];

        /// <summary>
        /// Attempts to construct the <see cref="SdkConfigurationSection"/> from the app.config file
        /// </summary>
        /// <param name="section">When the call returns in points to the created <see cref="SdkConfigurationSection"/></param>
        /// <returns>True if the <see cref="SdkConfigurationSection"/> was successfully constructed; False otherwise</returns>
        public static bool TryGetSection(out ISdkConfigurationSection section)
        {
            section = null;
            try
            {
                section =  GetSection();
                return true;
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is ConfigurationErrorsException)
                {
                    return false;
                }
                throw;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="SdkConfigurationSection"/> from the app.config file
        /// </summary>
        /// <returns>The <see cref="SdkConfigurationSection"/> instance loaded from config file</returns>
        /// <exception cref="InvalidOperationException">The configuration could not be loaded or the configuration does not contain the requested section</exception>
        /// <exception cref="ConfigurationErrorsException">The section in the configuration file is not valid</exception>
        public static ISdkConfigurationSection GetSection()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config == null)
            {
                throw new InvalidOperationException("Could not load exe configuration");
            }

            var section = (SdkConfigurationSection) ConfigurationManager.GetSection(SectionName);
            if (section == null)
            {
                throw new InvalidOperationException($"Could not retrieve section {SectionName} from exe configuration");
            }
            return section;
        }

        private string GetNullableString(string propertyName)
        {
            var property = ElementInformation.Properties[propertyName];
            return property?.ValueOrigin == PropertyValueOrigin.Default ? null : (string) property?.Value;
        }
    }
}