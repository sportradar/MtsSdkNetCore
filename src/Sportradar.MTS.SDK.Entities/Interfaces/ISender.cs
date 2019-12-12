/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.ComponentModel.DataAnnotations;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for a identification and settings of the ticket sender
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// Gets the ticket bookmaker id (client's id provided by Sportradar)
        /// </summary>
        [Required]
        int BookmakerId { get; }

        /// <summary>
        /// Gets the 3 letter currency code
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        string Currency { get; }

        /// <summary>
        /// Gets the client's limit id (provided by Sportradar to the client)
        /// </summary>
        [Required]
        int LimitId { get; }

        /// <summary>
        /// Gets the terminal id
        /// </summary>
        string TerminalId { get; }

        /// <summary>
        /// Gets the senders communication channel
        /// </summary>
        SenderChannel Channel { get; }

        /// <summary>
        /// Gets the shop id
        /// </summary>
        string ShopId { get; }

        /// <summary>
        /// Gets the identification of the end user (customer)
        /// </summary>
        IEndCustomer EndCustomer { get; }
    }
}