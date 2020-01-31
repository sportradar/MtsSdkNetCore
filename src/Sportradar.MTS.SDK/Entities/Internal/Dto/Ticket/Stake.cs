/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    internal partial class Stake
    {
        public Stake()
        {
        }

        public Stake(long value)
        {
            _value = value;
        }

        public Stake(long value, StakeType type)
        {
            _value = value;
            _type = type;
        }

        public Stake(IStake stake)
        {
            _value = stake.Value;
            _type = stake.Type.HasValue ? MtsTicketHelper.ConvertStakeType(stake.Type.Value) : (StakeType?)null;
        }
    }
}