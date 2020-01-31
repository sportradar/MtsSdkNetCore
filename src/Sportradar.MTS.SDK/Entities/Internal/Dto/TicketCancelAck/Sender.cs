/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck
{
    internal partial class Sender
    {
        public Sender()
        {
        }

        public Sender(int bookmakerId)
        {
            BookmakerId = bookmakerId;
        }
    }
}