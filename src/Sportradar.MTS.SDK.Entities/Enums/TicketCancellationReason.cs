/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Enum of possible reasons (codes) for ticket (bet) cancellation
    /// </summary>
    public enum TicketCancellationReason
    {
        /// <summary>
        /// The customer triggered prematch bet cancellation
        /// </summary>
        CustomerTriggeredPrematch = 101,

        /// <summary>
        /// The timeout triggered bet cancellation
        /// </summary>
        TimeoutTriggered = 102,

        /// <summary>
        /// The bookmaker backoffice triggered bet cancellation
        /// </summary>
        BookmakerBackofficeTriggered = 103,

        /// <summary>
        /// The bookmaker technical issue bet cancellation
        /// </summary>
        BookmakerTechnicalIssue = 104,

        /// <summary>
        /// The exceptional bookmaker triggered bet cancellation
        /// </summary>
        ExceptionalBookmakerTriggered = 105,

        /// <summary>
        /// The bookmaker cashback promotion cancellation
        /// </summary>
        BookmakerCashbackPromotionCancellation = 106,

        /// <summary>
        /// The sogei triggered bet cancellation
        /// </summary>
        SogeiTriggered = 301,

        /// <summary>
        /// The SCCS triggered bet cancellation
        /// </summary>
        SccsTriggered = 302
    }
}