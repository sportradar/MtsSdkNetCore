/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for classes implementing the bet bonus
    /// </summary>
    public interface IBetBonus
    {
        /// <summary>
        /// Gets the Quantity multiplied by 10000 and rounded to a long value
        /// </summary>
        long Value { get; }

        /// <summary>
        /// Gets the type of the bonus
        /// </summary>
        /// <value>(optional, default total)</value>
        BetBonusType Type { get; }

        /// <summary>
        /// Gets the Payout mode (optional, default proportional). Relevant mostly for system bets.
        /// Any: if at least one bet wins entire bonus is paid out.
        /// Proportional: paid out bonus is proportional to number of won bets.
        /// All: all bets must win for bonus to be paid out.
        /// </summary>
        BetBonusMode Mode { get; }
    }
}