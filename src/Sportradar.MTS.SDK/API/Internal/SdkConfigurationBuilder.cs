/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal;

namespace Sportradar.MTS.SDK.API.Internal
{
    internal class SdkConfigurationBuilder : ISdkConfigurationBuilder
    {
        private string _username;
        private string _password;
        private string _host;
        private string _vhost;
        private int _nodeId;
        private bool _useSsl;
        private int _bookmakerId;
        private int _limitId;
        private string _currency;
        private SenderChannel? _senderChannel;
        private string _accessToken;
        private UfEnvironment? _ufEnvironment;
        private bool _provideAdditionalMarketSpecifiers;
        private int _port;
        private bool _exclusiveConsumer;
        private string _keycloakHost;
        private string _keycloakUsername;
        private string _keycloakPassword;
        private string _keycloakSecret;
        private string _mtsClientApiHost;
        private int _ticketResponseTimeoutLive;
        private int _ticketResponseTimeoutPrematch;
        private int _ticketCancellationResponseTimeout;
        private int _ticketCashoutResponseTimeout;
        private int _ticketNonSrSettleResponseTimeout;

        internal SdkConfigurationBuilder()
        {
            _useSsl = true;
            _provideAdditionalMarketSpecifiers = true;
            _exclusiveConsumer = true;
            _ticketResponseTimeoutLive = SdkInfo.TicketResponseTimeoutLiveDefault;
            _ticketResponseTimeoutPrematch = SdkInfo.TicketResponseTimeoutPrematchDefault;
            _ticketCancellationResponseTimeout = SdkInfo.TicketCancellationResponseTimeoutDefault;
            _ticketCashoutResponseTimeout = SdkInfo.TicketCashoutResponseTimeoutDefault;
            _ticketNonSrSettleResponseTimeout = SdkInfo.TicketNonSrResponseTimeoutDefault;
        }

        public ISdkConfigurationBuilder SetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(username));
            }
            _username = username;
            return this;
        }

        public ISdkConfigurationBuilder SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(password));
            }
            _password = password;
            return this;
        }

        public ISdkConfigurationBuilder SetHost(string host)
        {
            if (string.IsNullOrEmpty(host) || host.Contains(":"))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string and no port number allowed", nameof(host));
            }
            _host = host;
            return this;
        }

        public ISdkConfigurationBuilder SetPort(int port)
        {
            if (port < 1)
            {
                throw new ArgumentException("Not valid port number.");
            }
            _port = port;
            return this;
        }

        public ISdkConfigurationBuilder SetVirtualHost(string vhost)
        {
            if (string.IsNullOrEmpty(vhost))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(vhost));
            }
            _vhost = vhost;
            return this;
        }

        public ISdkConfigurationBuilder SetNode(int nodeId)
        {
            if (nodeId < 1)
            {
                throw new ArgumentException("Value must be greater then zero", nameof(nodeId));
            }
            _nodeId = nodeId;
            return this;
        }

        public ISdkConfigurationBuilder SetUseSsl(bool useSsl)
        {
            _useSsl = useSsl;
            return this;
        }

        public ISdkConfigurationBuilder SetBookmakerId(int bookmakerId)
        {
            if (bookmakerId < 1)
            {
                throw new ArgumentException("Value must be greater then zero", nameof(bookmakerId));
            }
            _bookmakerId = bookmakerId;
            return this;
        }

        public ISdkConfigurationBuilder SetLimitId(int limitId)
        {
            if (limitId < 1)
            {
                throw new ArgumentException("Value must be greater then zero", nameof(limitId));
            }
            _limitId = limitId;
            return this;
        }

        public ISdkConfigurationBuilder SetCurrency(string currency)
        {
            if (string.IsNullOrEmpty(currency) || currency.Length < 3 || currency.Length > 4)
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string and length 3 or 4", nameof(currency));
            }
            _currency = currency;
            return this;
        }

        public ISdkConfigurationBuilder SetSenderChannel(SenderChannel channel)
        {
            _senderChannel = channel;
            return this;
        }

        public ISdkConfigurationBuilder SetAccessToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(accessToken));
            }
            _accessToken = accessToken;
            return this;
        }

        /// <summary>
        /// Sets the uf environment
        /// </summary>
        /// <param name="ufEnvironment">The uf environment to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        public ISdkConfigurationBuilder SetUfEnvironment(UfEnvironment ufEnvironment)
        {
            _ufEnvironment = ufEnvironment;
            return this;
        }

        public ISdkConfigurationBuilder SetProvideAdditionalMarketSpecifiers(bool provideAdditionalMarketSpecifiers)
        {
            _provideAdditionalMarketSpecifiers = provideAdditionalMarketSpecifiers;
            return this;
        }

        /// <summary>
        /// Sets the value indicating whether the rabbit consumer channel should be exclusive
        /// </summary>
        /// <param name="exclusiveConsumer">The value to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        public ISdkConfigurationBuilder SetExclusiveConsumer(bool exclusiveConsumer)
        {
            _exclusiveConsumer = exclusiveConsumer;
            return this;
        }

        public ISdkConfigurationBuilder SetKeycloakHost(string keycloakHost)
        {
            if (string.IsNullOrEmpty(keycloakHost))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(keycloakHost));
            }
            _keycloakHost = keycloakHost;
            return this;
        }

        public ISdkConfigurationBuilder SetKeycloakUsername(string keycloakUsername)
        {
            if (string.IsNullOrEmpty(keycloakUsername))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(keycloakUsername));
            }
            _keycloakUsername = keycloakUsername;
            return this;
        }

        public ISdkConfigurationBuilder SetKeycloakPassword(string keycloakPassword)
        {
            if (string.IsNullOrEmpty(keycloakPassword))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(keycloakPassword));
            }
            _keycloakPassword = keycloakPassword;
            return this;
        }

        public ISdkConfigurationBuilder SetKeycloakSecret(string keycloakSecret)
        {
            if (string.IsNullOrEmpty(keycloakSecret))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(keycloakSecret));
            }
            _keycloakSecret = keycloakSecret;
            return this;
        }

        public ISdkConfigurationBuilder SetMtsClientApiHost(string mtsClientApiHost)
        {
            if (string.IsNullOrEmpty(mtsClientApiHost))
            {
                throw new ArgumentException("Value cannot be a null reference or an empty string", nameof(mtsClientApiHost));
            }
            _mtsClientApiHost = mtsClientApiHost;
            return this;
        }

        public ISdkConfigurationBuilder SetTicketResponseTimeout(int responseTimeout)
        {
            if (responseTimeout < SdkInfo.TicketResponseTimeoutPrematchMin)
            {
                throw new ArgumentException($"responseTimeout must be more than {SdkInfo.TicketResponseTimeoutPrematchMin}ms", nameof(responseTimeout));
            }
            if (responseTimeout > SdkInfo.TicketResponseTimeoutLiveMax)
            {
                throw new ArgumentException($"responseTimeout must be less than {SdkInfo.TicketResponseTimeoutLiveMax}ms", nameof(responseTimeout));
            }
            _ticketResponseTimeoutLive = responseTimeout;
            _ticketResponseTimeoutPrematch = responseTimeout;
            return this;
        }

        /// <summary>
        /// Sets the ticket response timeout(ms)
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        public ISdkConfigurationBuilder SetTicketResponseTimeoutLive(int responseTimeout)
        {
            if (responseTimeout < SdkInfo.TicketResponseTimeoutLiveMin)
            {
                throw new ArgumentException($"responseTimeout must be more than {SdkInfo.TicketResponseTimeoutLiveMin}ms", nameof(responseTimeout));
            }
            if (responseTimeout > SdkInfo.TicketResponseTimeoutLiveMax)
            {
                throw new ArgumentException($"responseTimeout must be less than {SdkInfo.TicketResponseTimeoutLiveMax}ms", nameof(responseTimeout));
            }
            _ticketResponseTimeoutLive = responseTimeout;
            return this;
        }

        /// <summary>
        /// Sets the ticket response timeout(ms)
        /// </summary>
        /// <param name="responseTimeout">The timeout in ms to be set</param>
        /// <returns>Returns a <see cref="ISdkConfigurationBuilder"/></returns>
        public ISdkConfigurationBuilder SetTicketResponseTimeoutPrematch(int responseTimeout)
        {
            if (responseTimeout < SdkInfo.TicketResponseTimeoutPrematchMin)
            {
                throw new ArgumentException($"responseTimeout must be more than {SdkInfo.TicketResponseTimeoutPrematchMin}ms", nameof(responseTimeout));
            }
            if (responseTimeout > SdkInfo.TicketResponseTimeoutPrematchMax)
            {
                throw new ArgumentException($"responseTimeout must be less than {SdkInfo.TicketResponseTimeoutPrematchMax}ms", nameof(responseTimeout));
            }
            _ticketResponseTimeoutPrematch = responseTimeout;
            return this;
        }

        public ISdkConfigurationBuilder SetTicketCancellationResponseTimeout(int responseTimeout)
        {
            if (responseTimeout < SdkInfo.TicketCancellationResponseTimeoutMin)
            {
                throw new ArgumentException($"responseTimeout must be more than {SdkInfo.TicketCancellationResponseTimeoutMin}ms", nameof(responseTimeout));
            }
            if (responseTimeout > SdkInfo.TicketCancellationResponseTimeoutMax)
            {
                throw new ArgumentException($"responseTimeout must be less than {SdkInfo.TicketCancellationResponseTimeoutMax}ms", nameof(responseTimeout));
            }
            _ticketCancellationResponseTimeout = responseTimeout;
            return this;
        }

        public ISdkConfigurationBuilder SetTicketCashoutResponseTimeout(int responseTimeout)
        {
            if (responseTimeout < SdkInfo.TicketCashoutResponseTimeoutMin)
            {
                throw new ArgumentException($"responseTimeout must be more than {SdkInfo.TicketCashoutResponseTimeoutMin}ms", nameof(responseTimeout));
            }
            if (responseTimeout > SdkInfo.TicketCashoutResponseTimeoutMax)
            {
                throw new ArgumentException($"responseTimeout must be less than {SdkInfo.TicketCashoutResponseTimeoutMax}ms", nameof(responseTimeout));
            }
            _ticketCashoutResponseTimeout = responseTimeout;
            return this;
        }

        public ISdkConfigurationBuilder SetNonSrSettleResponseTimeout(int responseTimeout)
        {
            if (responseTimeout < SdkInfo.TicketCashoutResponseTimeoutMin)
            {
                throw new ArgumentException($"responseTimeout must be more than {SdkInfo.TicketCashoutResponseTimeoutMin}ms", nameof(responseTimeout));
            }
            if (responseTimeout > SdkInfo.TicketCashoutResponseTimeoutMax)
            {
                throw new ArgumentException($"responseTimeout must be less than {SdkInfo.TicketCashoutResponseTimeoutMax}ms", nameof(responseTimeout));
            }
            _ticketNonSrSettleResponseTimeout = responseTimeout;
            return this;
        }

        public ISdkConfiguration Build()
        {
            if (string.IsNullOrEmpty(_username))
            {
                throw new ArgumentException("Username cannot be a null reference or an empty string");
            }
            if (string.IsNullOrEmpty(_password))
            {
                throw new ArgumentException("Password cannot be a null reference or an empty string");
            }
            if (string.IsNullOrEmpty(_host))
            {
                throw new ArgumentException("Host cannot be a null reference or an empty string");
            }
            return new SdkConfiguration(_username,
                                        _password,
                                        _host,
                                        _vhost,
                                        _useSsl,
                                        _nodeId,
                                        _bookmakerId,
                                        _limitId,
                                        _currency,
                                        _senderChannel,
                                        _accessToken,
                                        _ufEnvironment,
                                        _provideAdditionalMarketSpecifiers,
                                        _port,
                                        _exclusiveConsumer,
                                        _keycloakHost,
                                        _keycloakUsername,
                                        _keycloakPassword,
                                        _keycloakSecret,
                                        _mtsClientApiHost,
                                        _ticketResponseTimeoutLive,
                                        _ticketResponseTimeoutPrematch,
                                        _ticketCancellationResponseTimeout,
                                        _ticketCashoutResponseTimeout,
                                        _ticketNonSrSettleResponseTimeout);
        }
    }
}
