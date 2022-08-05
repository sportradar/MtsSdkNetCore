/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    internal class Stake : IStake
    {
        public long Value { get; }
        public StakeType? Type { get; }

        [JsonConstructor]
        private Stake(long value, StakeType? type)
        {
            Value = value;
            Type = type;
        }

        public Stake(long value)
        {
            Guard.Argument(value, nameof(value)).InRange(0, 1000000000000000000);

            Value = value;
        }

        public Stake(long value, StakeType type)
        {
            Guard.Argument(value, nameof(value)).InRange(0, 1000000000000000000);

            Value = value;
            Type = type;
        }
    }
}