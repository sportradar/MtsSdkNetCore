/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Threading.Tasks;

using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.API
{
    /// <summary>
    /// Defines a contract used to send requests to MTS Client REST API
    /// </summary>

    public interface IMtsClientApi
    {
        /// <summary>
        /// Gets maximum stake for a ticket
        /// </summary>
        /// <param name="ticket">A <see cref="ITicket"/> to be send</param>
        /// <returns>Maximum reoffer stake (quantity multiplied by 10000 and rounded to a long value)</returns>
        Task<long> GetMaxStakeAsync(ITicket ticket);

        /// <summary>
        /// Gets maximum stake for a ticket
        /// </summary>
        /// <param name="ticket">A <see cref="ITicket"/> to be send</param>
        /// <param name="username">A username used for authentication</param>
        /// <param name="password">A password used for authentication</param>
        /// <returns>Maximum reoffer stake (quantity multiplied by 10000 and rounded to a long value)</returns>
        Task<long> GetMaxStakeAsync(ITicket ticket, string username, string password);

        /// <summary>
        /// Gets customer confidence factor for a customer
        /// </summary>
        /// <param name="sourceId">A source ID which identifies a customer</param>
        /// <returns>A <see cref="ICcf"/> values for sport and prematch/live (if set for customer)</returns>
        Task<ICcf> GetCcfAsync(string sourceId);

        /// <summary>
        /// Gets customer confidence factor for a customer
        /// </summary>
        /// <param name="sourceId">A source ID which identifies a customer</param>
        /// <param name="username">A username used for authentication</param>
        /// <param name="password">A password used for authentication</param>
        /// <returns>A <see cref="ICcf"/> values for sport and prematch/live (if set for customer)</returns>
        Task<ICcf> GetCcfAsync(string sourceId, string username, string password);
    }
}