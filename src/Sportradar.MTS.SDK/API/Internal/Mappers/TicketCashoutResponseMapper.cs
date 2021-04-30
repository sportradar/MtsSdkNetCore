/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.API.Internal.TicketImpl;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashoutResponse;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    /// <summary>
    /// Implementation of <see cref="ITicketMapper{TIn,TOut}"/> for <see cref="ITicketCashoutResponse"/>
    /// </summary>
    internal class TicketCashoutResponseMapper : ITicketResponseMapper<TicketCashoutResponseDTO, ITicketCashoutResponse>
    {
        /// <summary>
        /// Maps the specified source
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="correlationId">The correlation id of the response</param>
        /// <param name="additionalInfo">The additional information</param>
        /// <param name="orgJson">The original json string received from the mts</param>
        /// <returns>A <see cref="ITicketCashoutResponse"/></returns>
        public ITicketCashoutResponse Map(TicketCashoutResponseDTO source, string correlationId, IDictionary<string, string> additionalInfo, string orgJson)
        {
            return new TicketCashoutResponse(source.Result.TicketId,
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