/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.API.Internal.Mappers
{
    /// <summary>
    /// Defines a contract for mapping between user exposes entities and DTOs
    /// </summary>
    /// <typeparam name="TIn">The type of the t in</typeparam>
    /// <typeparam name="TOut">The type of the t out</typeparam>
    internal interface ITicketMapper<in TIn, out TOut>
    {
        /// <summary>
        /// Maps the specified source
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>TOut</returns>
        TOut Map(TIn source);
    }
}
