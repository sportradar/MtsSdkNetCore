/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Concurrent;
using Dawn;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.API.Internal.RabbitMq;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck;

namespace Sportradar.MTS.SDK.API.Internal.Senders
{
    internal class TicketCancelAckSender : TicketSenderBase
    {
        private readonly ITicketMapper<ITicketCancelAck, TicketCancelAckDTO> _ticketMapper;

        internal TicketCancelAckSender(ITicketMapper<ITicketCancelAck, TicketCancelAckDTO> ticketMapper,
                              IRabbitMqPublisherChannel publisherChannel,
                              ConcurrentDictionary<string, TicketCacheItem> ticketCache,
                              IMtsChannelSettings mtsChannelSettings,
                              IRabbitMqChannelSettings rabbitMqChannelSettings)
            : base(publisherChannel, ticketCache, mtsChannelSettings, rabbitMqChannelSettings)
        {
            Guard.Argument(ticketMapper, nameof(ticketMapper)).NotNull();

            _ticketMapper = ticketMapper;
        }

        protected override string GetMappedDtoJsonMsg(ISdkTicket sdkTicket)
        {
            var ticket = sdkTicket as ITicketCancelAck;
            var dto = _ticketMapper.Map(ticket);
            return dto.ToJson();
        }
    }
}
