/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    internal partial class FreeStake
    {
        public FreeStake()
        { }

        public FreeStake(long value)
        {
            _value = value;
        }

        public FreeStake(long value, FreeStakeType type, FreeStakeDescription description, FreeStakePaidAs paidAs)
        {
            _value = value;
            _type = type;
            _description = description;
            _paidAs = paidAs;
        }

        public FreeStake(IFreeStake freeStake)
        {
            _value = freeStake.Value;
            _type = MtsTicketHelper.Convert(freeStake.Type);
            _description = MtsTicketHelper.Convert(freeStake.Description);
            _paidAs = MtsTicketHelper.Convert(freeStake.PaidAs);
        }
    }
}
