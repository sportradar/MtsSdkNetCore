/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using System.Net;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of <see cref="ISenderBuilder"/> for creating <see cref="ISender"/>
    /// </summary>
    /// <seealso cref="ISenderBuilder" />
    public class SenderBuilder : ISenderBuilder
    {
        /// <summary>
        /// The bookmaker identifier
        /// </summary>
        private int _bookmakerId;

        /// <summary>
        /// The currency
        /// </summary>
        private string _currency;

        /// <summary>
        /// The terminal identifier
        /// </summary>
        private string _terminalId;

        /// <summary>
        /// The channel
        /// </summary>
        private SenderChannel _channel;

        /// <summary>
        /// The shop identifier
        /// </summary>
        private string _shopId;

        /// <summary>
        /// The customer
        /// </summary>
        private IEndCustomer _customer;

        /// <summary>
        /// The limit identifier
        /// </summary>
        private int _limitId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SenderBuilder"/> class
        /// </summary>
        /// <param name="config">The <see cref="ISdkConfiguration"/> providing default builder values</param>
        internal SenderBuilder(ISdkConfiguration config)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            _bookmakerId = config.BookmakerId;
            _limitId = config.LimitId;
            _currency = config.Currency;
            if (config.Channel != null)
            {
                _channel = (SenderChannel)config.Channel;
            }
        }

        #region Obsolete_Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SenderBuilder"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <param name="limitId">The limit identifier</param>
        /// <param name="currency">The currency</param>
        /// <param name="channel">The channel</param>
        [Obsolete]
        internal SenderBuilder(int bookmakerId = 0,
                             int limitId = 0,
                             string currency = null,
                             SenderChannel channel = SenderChannel.Internet)
        {
            _bookmakerId = bookmakerId;
            _limitId = limitId;
            _currency = currency;
            _channel = channel;
        }

        /// <summary>
        /// The <see cref="ISdkConfigurationSection"/> loaded from app.config
        /// </summary>
        [Obsolete]
        private static ISdkConfigurationSection _section;

        /// <summary>
        /// Value indicating whether an attempt to load the <see cref="ISdkConfigurationSection"/> was already made
        /// </summary>
        [Obsolete]
        private static bool _sectionLoaded;

        /// <summary>
        /// Creates the <see cref="ISenderBuilder"/>
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <param name="limitId">The limit identifier</param>
        /// <param name="currency">The currency</param>
        /// <param name="channel">The channel</param>
        /// <returns>ISenderBuilder</returns>
        [Obsolete("Method Create(...) is obsolete. Please use the appropriate method on IBuilderFactory interface which can be obtained through MtsSdk instance")]
        public static ISenderBuilder Create(int bookmakerId = 0,
                                            int limitId = 0,
                                            string currency = null,
                                            SenderChannel? channel = null)
        {
            var tempChannel = SenderChannel.Internet;
            try
            {
                if (!_sectionLoaded)
                {
                    SdkConfigurationSection.TryGetSection(out _section);
                    _sectionLoaded = true;
                }

                if (_section != null)
                {
                    if (bookmakerId == 0)
                    {
                        bookmakerId = _section.BookmakerId;
                    }
                    if (limitId == 0)
                    {
                        limitId = _section.LimitId;
                    }
                    if (string.IsNullOrEmpty(currency))
                    {
                        currency = _section.Currency;
                    }
                    if (_section.Channel != null)
                    {
                        tempChannel = (SenderChannel) _section.Channel;
                    }
                }
                if (channel != null)
                {
                    tempChannel = (SenderChannel) channel;
                }

            }
            catch (Exception)
            {
                // ignored
            }
            return new SenderBuilder(bookmakerId, limitId, currency, tempChannel);
        }
        #endregion

        /// <summary>
        /// Sets the ticket bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetBookmakerId(int bookmakerId)
        {
            _bookmakerId = bookmakerId;
            ValidateData(false, true);
            return this;
        }

        /// <summary>
        /// Sets the 3 letter currency code
        /// </summary>
        /// <param name="currency">The currency</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetCurrency(string currency)
        {
            _currency = currency;
            ValidateData(false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the terminal id
        /// </summary>
        /// <param name="terminalId">The terminal id</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetTerminalId(string terminalId)
        {
            _terminalId = terminalId;
            ValidateData(false, false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the senders communication channel
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetSenderChannel(SenderChannel channel)
        {
            _channel = channel;
            return this;
        }

        /// <summary>
        /// Sets the shop id
        /// </summary>
        /// <param name="shopId">The shop id</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetShopId(string shopId)
        {
            _shopId = shopId;
            ValidateData(false, false, false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the client's limit id (provided by Sportradar to the client)
        /// </summary>
        /// <param name="limitId">The limit id</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetLimitId(int limitId)
        {
            _limitId = limitId;
            ValidateData(false, false, false, false, false, true);
            return this;
        }

        /// <summary>
        /// Set the identification of the end user (customer)
        /// </summary>
        /// <param name="customer">The end customer to be set</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetEndCustomer(IEndCustomer customer)
        {
            _customer = customer ?? throw new ArgumentException("Customer not valid.");
            return this;
        }

        /// <summary>
        /// Set the identification of the end user (customer)
        /// </summary>
        /// <param name="ip">The ip address of the end customer</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="languageId">The language id</param>
        /// <param name="deviceId">The device id</param>
        /// <param name="confidence">The confidence</param>
        /// <returns>Returns a <see cref="ISenderBuilder" /></returns>
        public ISenderBuilder SetEndCustomer(IPAddress ip, string customerId, string languageId, string deviceId, long confidence)
        {
            _customer = new EndCustomerBuilder().SetIp(ip).SetLanguageId(languageId).SetDeviceId(deviceId).SetId(customerId).SetConfidence(confidence).Build();
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ISender" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ISender Build()
        {
            ValidateData(true);
            return new Sender(_bookmakerId, _currency, _terminalId, _channel, _shopId, _customer, _limitId);
        }

        private void ValidateData(bool all = false, bool bookmakerId = false, bool currency = false, bool terminalId = false, bool shopId = false, bool limitId = false)
        {
            if (all || bookmakerId)
            {
                if (_bookmakerId <= 0)
                {
                    throw new ArgumentException("BookmakerId not valid.");
                }
            }
            if (all || currency)
            {
                if (string.IsNullOrEmpty(_currency) || _currency.Length < 3 || _currency.Length > 4)
                {
                    throw new ArgumentException("Currency not valid.");
                }
            }
            if (all || terminalId)
            {
                if (!string.IsNullOrEmpty(_terminalId) && !TicketHelper.ValidateUserId(_terminalId))
                {
                    throw new ArgumentException("TerminalId not valid.");
                }
            }
            if (all || shopId)
            {
                if (!string.IsNullOrEmpty(_shopId) && !TicketHelper.ValidateUserId(_shopId))
                {
                    throw new ArgumentException("ShopId not valid.");
                }
            }
            if (all || limitId)
            {
                if (_limitId <= 0)
                {
                    throw new ArgumentException("LimitId not valid.", nameof(limitId));
                }
            }
        }
    }
}