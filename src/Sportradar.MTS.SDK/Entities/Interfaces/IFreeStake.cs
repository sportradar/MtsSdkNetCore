/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{/// <summary>
 /// Defines a contract for classes implementing the free stake
 /// </summary>
    public interface IFreeStake
    {
        /// <summary>
        /// Gets the Quantity multiplied by 10000 and rounded to a long value
        /// </summary>
        long Value { get; }

        /// <summary>
        /// Gets the type of the free stake
        /// </summary>
        /// <value>(optional, default total)</value>
        FreeStakeType Type { get; }

        /// <summary>
        /// The field description is optional. Clients will choose one of the pre-defined types:
        /// "freeBet" – default value, assumed if missing in the ticket
        /// "rollover"
        /// "moneyBack"
        /// "oddsBooster"
        /// "partialFreeBet"
        /// "other"
        /// </summary>
        FreeStakeDescription Description { get; }

        /// <summary>
        /// The field PaidAs is optional, description of the bonus payment type:
        /// "cash" – default value, assumed if missing in the ticket
        /// "freeBet"
        /// </summary>
        FreeStakePaidAs PaidAs { get; }
    }
}
