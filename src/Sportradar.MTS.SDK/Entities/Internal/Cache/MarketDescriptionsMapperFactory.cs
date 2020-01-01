/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    /// <summary>
    /// Class MarketDescriptionsMapperFactory
    /// </summary>
    internal class MarketDescriptionsMapperFactory : ISingleTypeMapperFactory<market_descriptions, IEnumerable<MarketDescriptionDTO>>
    {
        /// <summary>
        /// Creates and returns an instance of Mapper for mapping <see cref="market_descriptions"/>
        /// </summary>
        /// <param name="data">A input instance which the created <see cref="MarketDescriptionsMapper"/> will map</param>
        /// <returns>New <see cref="MarketDescriptionsMapper" /> instance</returns>
        public ISingleTypeMapper<IEnumerable<MarketDescriptionDTO>> CreateMapper(market_descriptions data)
        {
            return new MarketDescriptionsMapper(data);
        }
    }
}