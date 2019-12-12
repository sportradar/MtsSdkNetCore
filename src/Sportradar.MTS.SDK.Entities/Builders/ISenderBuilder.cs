/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Net;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes implementing builder for <see cref="ISender" />
    /// </summary>
    public interface ISenderBuilder
    {
        /// <summary>
        /// Sets the ticket bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetBookmakerId(int bookmakerId);

        /// <summary>
        /// Sets the 3 letter currency code
        /// </summary>
        /// <param name="currency">The currency</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetCurrency(string currency);

        /// <summary>
        /// Sets the terminal id
        /// </summary>
        /// <param name="terminalId">The terminal id</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetTerminalId(string terminalId);

        /// <summary>
        /// Sets the senders communication channel
        /// </summary>
        /// <param name="channel">The channel</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetSenderChannel(SenderChannel channel);

        /// <summary>
        /// Sets the shop id
        /// </summary>
        /// <param name="shopId">The shop id</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetShopId(string shopId);

        /// <summary>
        /// Set the identification of the end user (customer)
        /// </summary>
        /// <param name="customer">The end customer to be set</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetEndCustomer(IEndCustomer customer);

        /// <summary>
        /// Set the identification of the end user (customer)
        /// </summary>
        /// <param name="ip">The ip address of the end customer</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="languageId">The language id</param>
        /// <param name="deviceId">The device id</param>
        /// <param name="confidence">The confidence</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetEndCustomer(IPAddress ip, string customerId, string languageId, string deviceId, long confidence);

        /// <summary>
        /// Sets the client's limit id (provided by Sportradar to the client)
        /// </summary>
        /// <param name="limitId">The limit id</param>
        /// <returns>Returns a <see cref="ISenderBuilder"/></returns>
        ISenderBuilder SetLimitId(int limitId);

        /// <summary>
        /// Builds the <see cref="ISender" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ISender Build();
    }
}
