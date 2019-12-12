/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    /// <summary>
    /// Defines a contract for mapping between user exposes entities and DTOs
    /// </summary>
    /// <typeparam name="TIn">The type of the t in</typeparam>
    /// <typeparam name="TOut">The type of the t out</typeparam>
    public interface ITicketResponseMapper<in TIn, out TOut>
    {
        /// <summary>
        /// Maps the specified source
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="correlationId">The correlation id</param>
        /// <param name="additionalInfo">The additional information</param>
        /// <param name="orgJson">The original json string received from the mts</param>
        /// <returns>TOut</returns>
        TOut Map(TIn source, string correlationId, IDictionary<string, string> additionalInfo, string orgJson);
    }
}
