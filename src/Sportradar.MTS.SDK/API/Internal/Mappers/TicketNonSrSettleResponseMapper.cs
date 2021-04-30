/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.API.Internal.TicketImpl;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettleResponse;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    /// <summary>
    /// Implementation of <see cref="ITicketMapper{TIn,TOut}"/> for <see cref="ITicketNonSrSettleResponse"/>
    /// </summary>
    internal class TicketNonSrSettleResponseMapper : ITicketResponseMapper<TicketNonSrSettleResponseDTO, ITicketNonSrSettleResponse>
    {
        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="correlationId">The correlation id of the response</param>
        /// <param name="orgJson">The original json string received from the mts</param>
        /// <param name="additionalInfo">The additional information</param>
        /// <returns>TicketNonSrSettleResponse</returns>
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
    }
}