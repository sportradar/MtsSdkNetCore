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
    internal class BetBonus : IBetBonus
    {
        public long Value { get; }
        public BetBonusType Type { get; }
        public BetBonusMode Mode { get; }
        public BetBonusDescription Description { get; }
        public BetBonusPaidAs PaidAs { get; }

        [JsonConstructor]
        public BetBonus(long value, BetBonusType type, BetBonusMode mode, BetBonusDescription? description = null, BetBonusPaidAs? paidAs = null)
        {
            Guard.Argument(value, nameof(value)).InRange(1, 1000000000000000000 - 1);

            Value = value;
            Type = type;
            Mode = mode;
            Description = description != null ? (BetBonusDescription)description : BetBonusDescription.AccaBonus;
            PaidAs = paidAs != null ? (BetBonusPaidAs)paidAs : BetBonusPaidAs.Cash;
        }
    }
}