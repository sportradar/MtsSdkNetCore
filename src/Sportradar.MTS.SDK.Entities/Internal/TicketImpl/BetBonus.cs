/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    [Serializable]
    public class BetBonus : IBetBonus
    {
        public long Value { get; }
        public BetBonusType Type { get; }
        public BetBonusMode Mode { get; }


        [JsonConstructor]
        public BetBonus(long value, BetBonusType type, BetBonusMode mode)
        {
            Guard.Argument(value).InRange(1, 1000000000000000000 - 1);

            Value = value;
            Type = type;
            Mode = mode;
        }
    }
}