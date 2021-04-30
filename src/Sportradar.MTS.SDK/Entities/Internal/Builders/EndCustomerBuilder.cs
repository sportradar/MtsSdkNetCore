/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Net;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="IEndCustomerBuilder"/>
    /// </summary>
    /// <seealso cref="IEndCustomerBuilder" />
    internal class EndCustomerBuilder : IEndCustomerBuilder
    {
        /// <summary>
        /// The ip
        /// </summary>
        private IPAddress _ip;

        /// <summary>
        /// The language identifier
        /// </summary>
        private string _langId;

        /// <summary>
        /// The device identifier
        /// </summary>
        private string _deviceId;

        /// <summary>
        /// The client identifier
        /// </summary>
        private string _clientId;

        /// <summary>
        /// The confidence
        /// </summary>
        private long _confidence;

        /// <summary>
        /// Sets the end user's ip
        /// </summary>
        /// <param name="ip">The ip address to be set</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder" /></returns>
        public IEndCustomerBuilder SetIp(IPAddress ip)
        {
            _ip = ip;
            return this;
        }

        /// <summary>
        /// Sets the 2-letter ISO 639-1 language code
        /// </summary>
        /// <param name="languageId">The language</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder" /></returns>
        public IEndCustomerBuilder SetLanguageId(string languageId)
        {
            _langId = languageId.ToUpper();
            ValidateData(false, true);
            return this;
        }

        /// <summary>
        /// Sets the device end user's device id
        /// </summary>
        /// <param name="deviceId">The device identifier</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder" /></returns>
        public IEndCustomerBuilder SetDeviceId(string deviceId)
        {
            _deviceId = deviceId;
            ValidateData(false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the end user's unique id (in client's system)
        /// </summary>
        /// <param name="clientId">The client identifier</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder" /></returns>
        public IEndCustomerBuilder SetId(string clientId)
        {
            _clientId = clientId;
            ValidateData(false, false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the suggested CCF of the customer multiplied by 10000 and rounded to a long value
        /// </summary>
        /// <param name="confidence">The confidence to be set</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder" /></returns>
        public IEndCustomerBuilder SetConfidence(long confidence)
        {
            _confidence = confidence;
            ValidateData(false, false, false, false, true);
            return this;
        }

        /// <summary>
        /// Builds the <see cref="IEndCustomer" />
        /// </summary>
        /// <returns>Returns an <see cref="IEndCustomer" /></returns>
        public IEndCustomer Build()
        {
            ValidateData(true);
            return new EndCustomer(_ip, _langId, _deviceId, _clientId, _confidence);
        }

        private void ValidateData(bool all = false, bool langId = false, bool deviceId = false, bool clientId = false, bool confidence = false)
        {
            if ((all || langId) && !string.IsNullOrEmpty(_langId) && _langId.Length != 2)
            {
                throw new ArgumentException("LanguageId not valid.");
            }
            if ((all || deviceId) && !string.IsNullOrEmpty(_deviceId) && !TicketHelper.ValidateUserId(_deviceId))
            {
                throw new ArgumentException("Stake not valid.");
            }
            if ((all || clientId) && !string.IsNullOrEmpty(_clientId) && !TicketHelper.ValidateUserId(_clientId))
            {
                throw new ArgumentException("ClientId not valid.");
            }
            if ((all || confidence) && _confidence < 0)
            {
                throw new ArgumentException("Confidence not valid.");
            }
        }
    }
}
