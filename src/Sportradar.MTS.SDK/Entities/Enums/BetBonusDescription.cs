/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Defines possible values for <see cref="IBetBonus.Description"/>
    /// </summary>
    public enum BetBonusDescription
    {
        /// <summary>
        /// AccaBonus: default value, assumed if missing in the ticket)
        /// </summary>
        AccaBonus,
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
