/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Net;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes building <see cref="IEndCustomer"/>
    /// </summary>
    public interface IEndCustomerBuilder
    {
        /// <summary>
        /// Sets the end user's ip
        /// </summary>
        /// <param name="ip">The ip address to be set</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder"/></returns>
        IEndCustomerBuilder SetIp(IPAddress ip);

        /// <summary>
        /// Sets the 2-letter ISO 639-1 language code
        /// </summary>
        /// <param name="languageId">The language</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder"/></returns>
        IEndCustomerBuilder SetLanguageId(string languageId);

        /// <summary>
        /// Sets the device end user's device id
        /// </summary>
        /// <param name="deviceId">The device identifier</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder"/></returns>
        IEndCustomerBuilder SetDeviceId(string deviceId);

        /// <summary>
        /// Sets the end user's unique id (in client's system)
        /// </summary>
        /// <param name="clientId">The client identifier</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder"/></returns>
        IEndCustomerBuilder SetId(string clientId);

        /// <summary>
        /// Sets the suggested CCF of the customer multiplied by 10000 and rounded to a long value
        /// </summary>
        /// <param name="confidence">The confidence to be set</param>
        /// <returns>Returns a <see cref="IEndCustomerBuilder"/></returns>
        IEndCustomerBuilder SetConfidence(long confidence);

        /// <summary>
        /// Builds the <see cref="IEndCustomer" />
        /// </summary>
        /// <returns>Returns an <see cref="IEndCustomer"/></returns>
        IEndCustomer Build();
    }
}
