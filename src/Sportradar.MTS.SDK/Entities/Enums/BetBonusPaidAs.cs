/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Defines possible values for <see cref="IBetBonus.PaidAs"/>
    /// </summary>
    public enum BetBonusPaidAs
    {
        /// <summary>
        /// Cash: default value, assumed if missing in the ticket)
        /// </summary>
        Cash,
        /// <summary>
        /// FreeBet
        /// </summary>
        FreeBet
    }
}
