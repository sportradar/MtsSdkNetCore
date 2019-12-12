/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    public class RejectionInfo : IRejectionInfo
    {
        public string Id { get; }
        public string EventId { get; }
        public int? Odds { get; }

        public RejectionInfo(string id, string eventId, int? odds)
        {
            Id = id;
            EventId = eventId;
            Odds = odds;
        }
    }
}