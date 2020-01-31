/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashout
{
    /// <summary>
    /// Class Anonymous for BetCashout
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    internal partial class Anonymous
    {
        public Anonymous()
        { }

        public Anonymous(string betId, long stake, int? percent)
        {
            _id = betId;
            _cashoutStake = stake;
            _cashoutPercent = percent;
        }

        public Anonymous(IBetCashout betCashout)
        {
            _id = betCashout.BetId;
            _cashoutStake = betCashout.CashoutStake;
            _cashoutPercent = betCashout.CashoutPercent;
        }
    }
}
