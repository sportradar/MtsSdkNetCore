/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    public class ResponseReason : IResponseReason
    {
        public int Code { get; }
        public string Message { get; }
        public string InternalMessage { get; }

        public ResponseReason(int code, string message)
        {
            Code = code;
            Message = message;
            InternalMessage = null;
        }
    }
}