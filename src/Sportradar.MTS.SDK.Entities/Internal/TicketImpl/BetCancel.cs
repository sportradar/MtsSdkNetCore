/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    public class BetCancel : IBetCancel
    {
        /// <summary>
        /// Gets the id of the bet
        /// </summary>
        public string BetId { get; }

        /// <summary>
        /// Gets the cancel percent of the assigned bet
        /// </summary>
        /// <value>The cancel percent</value>
        public int? CancelPercent { get; }

        /// <summary>
        /// Construct the bet cashout
        /// </summary>
        /// <param name="betId">The bet id</param>
        /// <param name="cancelPercent">The cashout percent value of the assigned bet (quantity multiplied by 10_000 and rounded to a int value)</param>
        [JsonConstructor]
        public BetCancel(string betId, int? cancelPercent)
        {
            if (!TicketHelper.ValidateTicketId(betId))
            {
                throw new ArgumentException("BetId not valid.");
            }
            if (!TicketHelper.ValidatePercent(cancelPercent))
            {
                throw new ArgumentException("Percent not valid.");
            }

            BetId = betId;
            CancelPercent = cancelPercent;
        }
    }
}
