/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Interfaces.CustomBet;
using Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet;

namespace Sportradar.MTS.SDK.Entities.Internal.CustomBetImpl
{
    /// <summary>
    /// Implements methods used to access available selections for the event
    /// </summary>
    public class AvailableSelections : IAvailableSelections
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableSelections"/> class
        /// </summary>
        /// <param name="availableSelections">a <see cref="AvailableSelectionsDTO"/> representing the available selections</param>
        public AvailableSelections(AvailableSelectionsDTO availableSelections)
        {
            if (availableSelections == null)
            {
                throw new ArgumentNullException(nameof(availableSelections));
            }

            Event = availableSelections.Event;
            Markets = availableSelections.Markets.Select(m => new Market(m)).ToList().AsReadOnly();
        }

        public string Event { get; }
        public IEnumerable<IMarket> Markets { get; }
    }
}
