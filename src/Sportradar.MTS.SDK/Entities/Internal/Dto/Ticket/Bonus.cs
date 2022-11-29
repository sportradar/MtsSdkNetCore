/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    internal partial class Bonus
    {
        public Bonus()
        { }

        public Bonus(long value)
        {
            _value = value;
        }

        public Bonus(long value, BonusType type, BonusMode mode, BonusDescription description, BonusPaidAs paidAs)
        {
            _value = value;
            _type = type;
            _mode = mode;
            _description = description;
            _paidAs = paidAs;
        }

        public Bonus(IBetBonus bonus)
        {
            _value = bonus.Value;
            _type = MtsTicketHelper.Convert(bonus.Type);
            _mode = MtsTicketHelper.Convert(bonus.Mode);
            _description = MtsTicketHelper.Convert(bonus.Description);
            _paidAs = MtsTicketHelper.Convert(bonus.PaidAs);
        }
    }
}