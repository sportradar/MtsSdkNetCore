/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Internal.Enums;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    public class MarketMappingCacheItem
    {
        internal int ProductId { get; }

        internal URN SportId { get; }

        internal int MarketTypeId { get; }

        internal int? MarketSubTypeId { get; }

        internal string SovTemplate { get; }

        internal IEnumerable<OutcomeMappingCacheItem> OutcomeMappings { get; }

        protected MarketMappingCacheItem(MarketMappingDTO dto)
        {
            Guard.Argument(dto, nameof(dto)).NotNull();

            ProductId = dto.ProductId;
            SportId = dto.SportId;
            MarketTypeId = dto.MarketTypeId;
            MarketSubTypeId = dto.MarketSubTypeId;
            SovTemplate = dto.SovTemplate;

            if (dto.OutcomeMappings != null)
            {
                OutcomeMappings = dto.OutcomeMappings.Select(o => new OutcomeMappingCacheItem(o));
            }
        }

        /// <summary>
        /// Constructs and returns a <see cref="MarketMappingCacheItem"/> from the provided DTO
        /// </summary>
        /// <param name="dto">The <see cref="MarketMappingDTO"/> containing mapping data</param>
        /// <returns>The constructed <see cref="MarketMappingCacheItem"/></returns>
        public static MarketMappingCacheItem Build(MarketMappingDTO dto)
        {
            Guard.Argument(dto, nameof(dto)).NotNull();

            return new MarketMappingCacheItem(dto);
        }

        // ReSharper disable once UnusedParameter.Global
        internal void Merge(MarketMappingDTO dto)
        {
            // this type has no translatable properties for now, so merge is not required
            // and the method is only defined for consistency
        }
    }
}