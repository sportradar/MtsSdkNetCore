/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Entities.Internal;

namespace Sportradar.MTS.SDK.API.Internal.Senders
{
    internal interface ITicketSenderFactory : IOpenable
    {
        ITicketSender GetTicketSender(SdkTicketType ticketType);
    }
}
