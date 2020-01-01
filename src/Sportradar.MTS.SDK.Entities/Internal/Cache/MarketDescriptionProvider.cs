/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Dawn;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Exceptions;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    public sealed class MarketDescriptionProvider : IMarketDescriptionProvider
    {
        /// <summary>
        /// A <see cref="ILogger"/> instance for execution logging
        /// </summary>
        private static readonly ILogger ExecutionLog = SdkLoggerFactory.GetLogger(typeof(MarketDescriptionProvider));

        internal readonly IMarketDescriptionCache MarketDescriptionCache;

        private readonly IEnumerable<CultureInfo> _cultures;

        public MarketDescriptionProvider(IMarketDescriptionCache marketDescriptionCache, IEnumerable<CultureInfo> cultures)
        {
            Guard.Argument(marketDescriptionCache, nameof(marketDescriptionCache)).NotNull();
            var cultureInfos = cultures.ToList();
            Guard.Argument(cultureInfos, nameof(cultureInfos)).NotNull().NotEmpty();

            MarketDescriptionCache = marketDescriptionCache;
            _cultures = cultureInfos;
        }

        /// <summary>
        /// Asynchronously gets a <see cref="MarketDescriptionCacheItem" /> instance for the market specified by <code>id</code> and <code>specifiers</code>
        /// </summary>
        /// <param name="marketId">The market identifier</param>
        /// <param name="variant">A <see cref="string" /> specifying market selectionId or a null reference if market is invariant</param>
        /// <returns>A <see cref="Task{T}" /> representing the async retrieval operation</returns>
        /// <exception cref="CacheItemNotFoundException">The requested key was not found in the cache and could not be loaded</exception>
        public async Task<MarketDescriptionCacheItem> GetMarketDescriptorAsync(int marketId, string variant)
        {
            if (MarketDescriptionCache == null)
            {
                throw new CommunicationException("No AccessToken provided.", null, null);
            }

            var cacheItem = await MarketDescriptionCache.GetMarketDescriptorAsync(marketId, variant, _cultures).ConfigureAwait(false);

            if (cacheItem == null)
            {
                ExecutionLog.LogWarning($"No MarketDescription found for marketId {marketId}.");
            }
            return cacheItem;
        }
    }
}