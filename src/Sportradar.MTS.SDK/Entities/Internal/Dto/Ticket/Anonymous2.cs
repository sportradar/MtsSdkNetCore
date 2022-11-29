/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    /// <summary>
    /// Class for TicketSelection
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    internal partial class Anonymous2
    {
        public Anonymous2()
        {
        }

        public Anonymous2(string eventId, string id, int odds)
        {
            _eventId = eventId;
            _id = id;
            _odds = odds;
        }

        public Anonymous2(ISelection selection)
        {
            _eventId = selection.EventId;
            _id = selection.Id;
            _odds = selection.Odds;
            _boostedOdds = selection.BoostedOdds;
        }

        public override bool Equals(object obj)
        {
            if (obj is Anonymous2 sel && string.Equals(sel.Id, Id) && sel.EventId == EventId && sel.Odds == Odds)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table</returns>
        public override int GetHashCode()
        {
            var x = $"{EventId}-{Odds}-{Id}".GetHashCode();
            return x;
        }
    }
}