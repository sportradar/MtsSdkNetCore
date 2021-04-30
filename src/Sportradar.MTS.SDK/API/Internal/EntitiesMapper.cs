/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using System.Linq;
using Sportradar.MTS.SDK.API.Internal.Senders;
using Sportradar.MTS.SDK.API.Internal.TicketImpl;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashoutResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettleResponse;

namespace Sportradar.MTS.SDK.API.Internal
{
    internal class EntitiesMapper
    {
        private readonly ITicketSender _ticketAckSender;
        private readonly ITicketSender _ticketCancelAckSender;

        public EntitiesMapper(ITicketSender ticketAckSender, ITicketSender ticketCancelAckSender)
        {
            _ticketAckSender = ticketAckSender;
            _ticketCancelAckSender = ticketCancelAckSender;
        }

        public ITicketResponse Map(TicketResponseDTO source, string correlationId, IDictionary<string, string> additionalInfo, string orgJson)
        {
            var autoAcceptedOdds = source.AutoAcceptedOdds?.Select(s => new AutoAcceptedOdds(s.SelectionIndex, s.RequestedOdds, s.UsedOdds));
            return new TicketResponse(_ticketAckSender,
                                      source.Result.TicketId,
                                      MtsTicketHelper.Convert(source.Result.Status),
                                      new ResponseReason(source.Result.Reason.Code, source.Result.Reason.Message),
                                      source.Result.BetDetails?.ToList().ConvertAll(b => new BetDetail(b)),
                                      correlationId,
                                      source.Signature,
                                      source.ExchangeRate,
                                      source.Version,
                                      additionalInfo,
                                      autoAcceptedOdds,
                                      orgJson);
        }

        public ITicketCancelResponse Map(TicketCancelResponseDTO source, string correlationId, IDictionary<string, string> additionalInfo, string orgJson)
        {
            return new TicketCancelResponse(_ticketCancelAckSender,
                                      source.Result.TicketId,
                                      MtsTicketHelper.Convert(source.Result.Status),
                                      new ResponseReason(source.Result.Reason.Code, source.Result.Reason.Message),
                                      correlationId,
                                      source.Signature,
                                      source.Version,
                                      additionalInfo,
                                      orgJson);
        }

        public ITicketCashoutResponse Map(TicketCashoutResponseDTO source, string correlationId, IDictionary<string, string> additionalInfo, string orgJson)
        {
            return new TicketCashoutResponse(
                source.Result.TicketId,
                MtsTicketHelper.Convert(source.Result.Status),
                new ResponseReason(source.Result.Reason.Code, source.Result.Reason.Message),
                correlationId,
                source.Signature,
                source.Version,
                additionalInfo,
                orgJson);
        }

        public ITicketNonSrSettleResponse Map(TicketNonSrSettleResponseDTO source, string correlationId, IDictionary<string, string> additionalInfo, string orgJson)
        {
            return new TicketNonSrSettleResponse(
                source.Result.TicketId,
                MtsTicketHelper.Convert(source.Result.Status),
                new ResponseReason(source.Result.Reason.Code, source.Result.Reason.Message),
                correlationId,
                source.Signature,
                source.Version,
                additionalInfo,
                orgJson);
        }

        public ISdkTicket GetTicketResponseFromJson(string json, string routingKey, TicketResponseType type, string correlationId, IDictionary<string, string> additionalInfo)
        {
            //new
            if (type == TicketResponseType.Ticket)
            {
                var ticketDto = TicketResponseDTO.FromJson(json);
                return Map(ticketDto, correlationId, additionalInfo, json);
            }
            if (type == TicketResponseType.TicketCancel)
            {
                var cancelDto = TicketCancelResponseDTO.FromJson(json);
                return Map(cancelDto, correlationId, additionalInfo, json);
            }
            if (type == TicketResponseType.TicketCashout)
            {
                var cashoutDto = TicketCashoutResponseDTO.FromJson(json);
                return Map(cashoutDto, correlationId, additionalInfo, json);
            }
            if (type == TicketResponseType.TicketNonSrSettle)
            {
                var nonSrSettleDto = TicketNonSrSettleResponseDTO.FromJson(json);
                return Map(nonSrSettleDto, correlationId, additionalInfo, json);
            }

            //old
            if (!routingKey.Contains("cancel"))
            {
                var ticketDto = TicketResponseDTO.FromJson(json);
                return Map(ticketDto, correlationId, additionalInfo, json);
            }

            var cancel2Dto = TicketCancelResponseDTO.FromJson(json);
            return Map(cancel2Dto, correlationId, additionalInfo, json);
        }
    }
}
