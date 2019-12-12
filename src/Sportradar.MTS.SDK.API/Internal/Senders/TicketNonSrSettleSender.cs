/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Concurrent;
using Dawn;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettle;

namespace Sportradar.MTS.SDK.API.Internal.Senders
{
    public class TicketNonSrSettleSender : TicketSenderBase
    {
        private readonly ITicketMapper<ITicketNonSrSettle, TicketNonSrSettleDTO> _ticketMapper;

        internal TicketNonSrSettleSender(ITicketMapper<ITicketNonSrSettle, TicketNonSrSettleDTO> ticketMapper,
                              IRabbitMqPublisherChannel publisherChannel,
                              ConcurrentDictionary<string, TicketCacheItem> ticketCache,
                              IMtsChannelSettings mtsChannelSettings,
                              IRabbitMqChannelSettings rabbitMqChannelSettings)
            : base(publisherChannel, ticketCache, mtsChannelSettings, rabbitMqChannelSettings)
        {
            Guard.Argument(ticketMapper).NotNull();

            _ticketMapper = ticketMapper;
        }

        protected override string GetMappedDtoJsonMsg(ISdkTicket sdkTicket)
        {
            var ticket = sdkTicket as ITicketNonSrSettle;
            var dto = _ticketMapper.Map(ticket);
            return dto.ToJson();
        }
    }
}
