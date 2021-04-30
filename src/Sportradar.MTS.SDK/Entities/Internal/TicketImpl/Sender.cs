/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
// ReSharper disable UnusedMember.Local
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    internal class Sender : ISender
    {
        /// <summary>
        /// Gets the ticket bookmaker id (client's id provided by Sportradar)
        /// </summary>
        public int BookmakerId { get; }

        /// <summary>
        /// Gets the 3 letter currency code
        /// </summary>
        public string Currency { get; }

        /// <summary>
        /// Gets the terminal id
        /// </summary>
        public string TerminalId { get; }

        /// <summary>
        /// Gets the senders communication channel
        /// </summary>
        public SenderChannel Channel { get; }

        /// <summary>
        /// Gets the shop id
        /// </summary>
        public string ShopId { get; }

        /// <summary>
        /// Gets the identification of the end user (customer)
        /// </summary>
        public IEndCustomer EndCustomer { get; }

        /// <summary>
        /// Gets the client's limit id (provided by Sportradar to the client)
        /// </summary>
        public int LimitId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sender"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <param name="currency">The currency</param>
        /// <param name="terminalId">The terminal identifier</param>
        /// <param name="channel">The sender channel</param>
        /// <param name="shopId">The shop identifier</param>
        /// <param name="endCustomer">The customer</param>
        /// <param name="limitId">The limit identifier</param>
        [JsonConstructor]
        public Sender(int bookmakerId, string currency, string terminalId, SenderChannel channel, string shopId, IEndCustomer endCustomer, int limitId)
        {
            Guard.Argument(bookmakerId, nameof(bookmakerId)).Positive();
            Guard.Argument(currency, nameof(currency)).NotNull().NotEmpty();
            Guard.Argument(currency.Length, nameof(currency.Length)).InRange(3, 4);
            Guard.Argument(terminalId, nameof(terminalId)).Require(string.IsNullOrEmpty(terminalId) || TicketHelper.ValidateUserId(terminalId));
            Guard.Argument(shopId, nameof(shopId)).Require(string.IsNullOrEmpty(shopId) || TicketHelper.ValidateUserId(shopId));
            Guard.Argument(limitId, nameof(limitId)).Positive();

            BookmakerId = bookmakerId;
            Currency = currency.Length == 3 ? currency.ToUpper() : currency;
            TerminalId = terminalId;
            Channel = channel;
            ShopId = shopId;
            EndCustomer = endCustomer;
            LimitId = limitId;

            ValidateSenderData();
        }

        private void ValidateSenderData()
        {
            CheckArgument(BookmakerId > 0, "BookmakerId", "BookmakerId is invalid.");
            CheckArgument(LimitId > 0, "LimitId", "LimitId is invalid.");
            CheckArgument(!string.IsNullOrEmpty(Currency), "Currency", "Currency is invalid.");
        }

        // ReSharper disable once UnusedParameter.Local
        private static void CheckArgument(bool input, string paramName, string msg)
        {
            if (!input)
            {
                throw new ArgumentException(msg, paramName);
            }
        }
    }
}