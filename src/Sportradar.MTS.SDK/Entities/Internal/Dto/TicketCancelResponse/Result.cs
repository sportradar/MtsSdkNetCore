/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelResponse
{
    internal partial class Result
    {
        public Result()
        { }

        public Result(string ticketId, Status status)
        {
            _ticketId = ticketId;
            _status = status;
        }
    }
}