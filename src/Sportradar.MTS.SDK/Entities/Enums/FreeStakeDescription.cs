/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Defines possible values for <see cref="IFreeStake.Description"/>
    /// </summary>
    public enum FreeStakeDescription
    {
        /// <summary>
        /// FreeBet: default value, assumed if missing in the ticket)
        /// </summary>
        FreeBet,
        /// <summary>
        /// PartialFreeBet
        /// </summary>
        PartialFreeBet,
        /// <summary>
        /// Rollover
        /// </summary>
        Rollover,
        /// <summary>
        /// MoneyBack
        /// </summary>
        MoneyBack,
        /// <summary>
        /// OddsBooster
        /// </summary>
        OddsBooster,
        /// <summary>
        /// Other
        /// </summary>
        Other
    }
}
