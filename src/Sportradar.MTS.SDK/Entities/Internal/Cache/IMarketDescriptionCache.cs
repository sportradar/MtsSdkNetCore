/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Sportradar.MTS.SDK.Common.Exceptions;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    /// <summary>
    /// Defines a contract implemented by classes used to cache market descriptions
    /// </summary>
    internal interface IMarketDescriptionCache
    {
        /// <summary>
        /// Asynchronously gets a <see cref="MarketDescriptionCacheItem" /> instance for the market specified by <code>id</code> and <code>specifiers</code>
        /// </summary>
        /// <param name="marketId">The market identifier</param>
        /// <param name="variant">A <see cref="string" /> specifying market variant or a null reference if market is invariant</param>
        /// <param name="cultures">A <see cref="IEnumerable{CultureInfo}" /> specifying required translations</param>
        /// <returns>A <see cref="Task{T}" /> representing the async retrieval operation</returns>
        /// <exception cref="CacheItemNotFoundException">The requested key was not found in the cache and could not be loaded</exception>
        Task<MarketDescriptionCacheItem> GetMarketDescriptorAsync(int marketId, string variant, IEnumerable<CultureInfo> cultures);
    }
}