/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Internal.Enums;

namespace Sportradar.MTS.SDK.Entities.Internal.REST.Dto
{
    /// <summary>
    /// A data-transfer-object representing a market mapping
    /// </summary>
    public class MarketMappingDTO
    {
        private readonly int _typeId;
        private readonly int _subTypeId;

        internal int ProductId { get; }

        internal URN SportId { get; }

        internal int MarketTypeId => _typeId;

        internal int? MarketSubTypeId => _subTypeId;

        internal string SovTemplate { get; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        internal string ValidFor { get; }

        internal IEnumerable<OutcomeMappingDTO> OutcomeMappings { get; }

        internal MarketMappingDTO(mappingsMapping mapping)
        {
            Guard.Argument(mapping).NotNull();
            Guard.Argument(mapping.product_id).Positive();
            Guard.Argument(mapping.sport_id).NotNull().NotEmpty();
            Guard.Argument(mapping.market_id).NotNull().NotEmpty();

            ProductId = mapping.product_id;
            SportId = mapping.sport_id == "all" ? null : URN.Parse(mapping.sport_id);
            var marketId = mapping.market_id.Split(':');
            int.TryParse(marketId[0], out _typeId);
            if (marketId.Length == 2)
            {
                int.TryParse(marketId[1], out _subTypeId);
            }
            SovTemplate = mapping.sov_template;
            ValidFor = mapping.valid_for;

            if (mapping.mapping_outcome != null)
            {
                OutcomeMappings = mapping.mapping_outcome.Select(o => new OutcomeMappingDTO(o));
            }
        }
    }
}