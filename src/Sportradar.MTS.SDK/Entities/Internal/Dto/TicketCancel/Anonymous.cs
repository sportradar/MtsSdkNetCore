/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancel
{
    /// <summary>
    /// Class Anonymous for BetCancel
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public partial class Anonymous
    {
        public Anonymous()
        { }

        public Anonymous(string betId, int? percent)
        {
            _id = betId;
            _cancelPercent = percent;
        }

        public Anonymous(IBetCancel betCancel)
        {
            _id = betCancel.BetId;
            _cancelPercent = betCancel.CancelPercent;
        }
    }
}
