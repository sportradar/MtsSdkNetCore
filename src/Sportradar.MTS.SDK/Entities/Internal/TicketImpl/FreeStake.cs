/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using System;


namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    [Serializable]
    internal class FreeStake : IFreeStake
    {
        public long Value { get; }
        public FreeStakeType Type { get; }
        public FreeStakeDescription Description { get; }
        public FreeStakePaidAs PaidAs { get; }


        [JsonConstructor]
        public FreeStake(long value, FreeStakeType? type, FreeStakeDescription? description = null, FreeStakePaidAs? paidAs = null)
        {
            Guard.Argument(value, nameof(value)).InRange(0, 1000000000000000000);

            Value = value;
            Type = type != null ? (FreeStakeType)type : FreeStakeType.Total;
            Description = description != null ? (FreeStakeDescription)description : FreeStakeDescription.FreeBet;
            PaidAs = paidAs != null ? (FreeStakePaidAs)paidAs : FreeStakePaidAs.Cash;
        }
    }
}
