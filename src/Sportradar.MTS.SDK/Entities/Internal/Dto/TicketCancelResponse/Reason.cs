/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelResponse
{
    public partial class Reason
    {
        public Reason()
        { }

        public Reason(int code, string message)
        {
            _code = code;
            _message = message;
        }
    }
}