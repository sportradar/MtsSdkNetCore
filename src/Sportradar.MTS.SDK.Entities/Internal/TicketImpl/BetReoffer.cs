/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    public class BetReoffer : IBetReoffer
    {
        public BetReofferType Type { get; }

        public long Stake { get; }

        public BetReoffer(long stake, BetReofferType type)
        {
            Guard.Argument(stake, nameof(stake)).InRange(1, 1000000000000000000-1);

            Stake = stake;
            Type = type;
        }
    }
}