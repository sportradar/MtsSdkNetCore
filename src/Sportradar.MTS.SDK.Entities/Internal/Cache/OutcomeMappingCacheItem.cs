/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    internal class OutcomeMappingCacheItem
    {
        public string OutcomeId { get; }

        public string ProductOutcomeId { get; }

        public string ProductOutcomeName { get; }

        public OutcomeMappingCacheItem(OutcomeMappingDTO dto)
        {
            Guard.Argument(dto).NotNull();

            OutcomeId = dto.OutcomeId;
            ProductOutcomeId = dto.ProductOutcomeId;
            ProductOutcomeName = dto.ProductOutcomeName;
        }
    }
}