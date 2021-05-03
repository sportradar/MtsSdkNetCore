/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Internal.REST;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet
{
    /// <summary>
    /// Defines a data-transfer-object for available selections for the event
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Allowed")]
    internal class AvailableSelectionsDTO
    {
        /// <summary>
        /// Gets the <see cref="string"/> of the event
        /// </summary>
        public string Event { get; }

        /// <summary>
        /// Gets the list of markets for this event
        /// </summary>
        public IEnumerable<MarketDTO> Markets { get; }

        internal AvailableSelectionsDTO(AvailableSelectionsType availableSelections)
        {
            if (availableSelections == null)
                throw new ArgumentNullException(nameof(availableSelections));

            Event = availableSelections.@event.id;
            var markets = availableSelections.@event.markets;
            Markets = markets != null
                ? markets.Select(m => new MarketDTO(m)).ToList().AsReadOnly()
                : new ReadOnlyCollection<MarketDTO>(new List<MarketDTO>());
        }
    }
}
