/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Contract defining Ticket that can be send to the MTS
    /// </summary>
    public interface ITicket : ISdkTicket
    {
        /// <summary>
        /// Gets the collection of all bets
        /// </summary>
        [Required]
        IEnumerable<IBet> Bets { get; }

        /// <summary>
        /// Gets the identification and settings of the ticket sender
        /// </summary>
        [Required]
        ISender Sender { get; }

        /// <summary>
        /// Gets the reoffer reference ticket id
        /// </summary>
        string ReofferId { get; }

        /// <summary>
        /// Gets the alternative stake reference ticket id
        /// </summary>
        string AltStakeRefId { get; }

        /// <summary>
        /// Gets a value indicating whether this is for testing
        /// </summary>
        bool TestSource { get; }

        /// <summary>
        /// Gets the type of the odds change Accept change in odds (optional, default none)
        /// <see cref="OddsChangeType.None"/>: default behavior
        /// <see cref="OddsChangeType.Any"/>: any odds change accepted
        /// <see cref="OddsChangeType.Higher"/>: accept higher odds
        /// </summary>
        OddsChangeType? OddsChange { get; }

        /// <summary>
        /// Gets the collection of all selections
        /// </summary>
        [Required]
        IEnumerable<ISelection> Selections { get; }

        /// <summary>
        /// Gets the expected total number of generated combinations on this ticket (optional, default null). If present, it is used to validate against actual number of generated combinations.
        /// </summary>
        /// <value>The total combinations</value>
        int? TotalCombinations { get; }

        /// <summary>
        /// Gets end time of last (non Sportradar) match on ticket
        /// </summary>
        /// <value>End time of last (non Sportradar) match on ticket</value>
        DateTime? LastMatchEndTime { get; }

        /// <summary>
        /// Capped max payout of the ticket
        /// </summary>
        long? PayCap { get; }
    }
}