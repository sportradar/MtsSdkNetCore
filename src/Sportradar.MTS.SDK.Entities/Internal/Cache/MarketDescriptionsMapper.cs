/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    internal class MarketDescriptionsMapper : ISingleTypeMapper<IEnumerable<MarketDescriptionDTO>>
    {
        /// <summary>
        /// A <see cref="market_descriptions"/> instance containing data used to construct <see cref="IEnumerable{MarketDescriptionDTO}"/> instance
        /// </summary>
        private readonly market_descriptions _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDescriptionsMapper"/> class
        /// </summary>
        /// <param name="data">A <see cref="market_descriptions"/> instance containing data used to construct <see cref="IEnumerable{MarketDescriptionDTO}"/> instance</param>
        internal MarketDescriptionsMapper(market_descriptions data)
        {
            Guard.Argument(data).NotNull();

            _data = data;
        }

        /// <summary>
        /// Maps it's data to <see cref="IEnumerable{MarketDescriptionDTO}"/> instance
        /// </summary>
        /// <returns>The created<see cref="IEnumerable{MarketDescriptionDTO}"/> instance</returns>
        IEnumerable<MarketDescriptionDTO> ISingleTypeMapper<IEnumerable<MarketDescriptionDTO>>.Map()
        {
            var descriptions = _data.market.Select(m => new MarketDescriptionDTO(m)).ToList();
            return new List<MarketDescriptionDTO>(descriptions);
        }
    }
}