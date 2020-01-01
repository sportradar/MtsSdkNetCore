/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    internal class MarketSpecifierCacheItem
    {
        internal string Name { get; }

        internal string Type { get; }

        internal MarketSpecifierCacheItem(MarketSpecifierDTO dto)
        {
            Guard.Argument(dto, nameof(dto)).NotNull();

            Type = dto.Type;
            Name = dto.Name;
        }
    }

    internal class MarketAttributeCacheItem
    {
        internal string Name { get; }

        internal string Description { get; }

        internal MarketAttributeCacheItem(MarketAttributeDTO dto)
        {
            Guard.Argument(dto, nameof(dto)).NotNull();

            Name = dto.Name;
            Description = dto.Description;
        }
    }
}