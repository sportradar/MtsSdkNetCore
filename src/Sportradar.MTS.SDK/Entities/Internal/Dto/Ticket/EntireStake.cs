/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    public partial class EntireStake
    {
        public EntireStake()
        {
        }

        public EntireStake(long value)
        {
            _value = value;
        }

        public EntireStake(long value, EntireStakeType type)
        {
            _value = value;
            _type = type;
        }

        public EntireStake(IStake stake)
        {
            _value = stake.Value;
            _type = stake.Type.HasValue ? MtsTicketHelper.ConvertEntireStakeType(stake.Type.Value) : (EntireStakeType?)null;
        }
    }
}