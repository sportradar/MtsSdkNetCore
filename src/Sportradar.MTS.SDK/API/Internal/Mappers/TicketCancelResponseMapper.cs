/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.API.Internal.Senders;
using Sportradar.MTS.SDK.API.Internal.TicketImpl;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelResponse;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    /// <summary>
    /// Implementation of <see cref="ITicketMapper{TIn,TOut}"/> for <see cref="ITicketCancelResponse"/>
    /// </summary>
    internal class TicketCancelResponseMapper : ITicketResponseMapper<TicketCancelResponseDTO, ITicketCancelResponse>
    {
        /// <summary>
        /// The ticket cancel ack sender
        /// </summary>
        private readonly ITicketSender _ticketCancelAckSender;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCancelResponseMapper"/> class
        /// </summary>
        /// <param name="ticketCancelAckSender">The ticket cancel ack sender</param>
        public TicketCancelResponseMapper(ITicketSender ticketCancelAckSender)
        {
            _ticketCancelAckSender = ticketCancelAckSender;
        }

        /// <summary>
        /// Maps the specified source
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="correlationId">The correlation id of the response</param>
        /// <param name="additionalInfo">The additional info regarding this cancel response</param>
        /// <param name="orgJson">The original json string received from the mts</param>
        /// <returns>ITicketCancelResponse</returns>
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
    }
}