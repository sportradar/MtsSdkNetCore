/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Threading.Tasks;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    public interface IMarketDescriptionProvider
    {
        Task<MarketDescriptionCacheItem> GetMarketDescriptorAsync(int marketId, string variant);
    }
}