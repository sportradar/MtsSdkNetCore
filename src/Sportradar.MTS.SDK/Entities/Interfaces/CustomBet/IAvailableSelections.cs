/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.Entities.Interfaces.CustomBet
{
    /// <summary>
    /// Provides an available selections for a particular event
    /// </summary>
    public interface IAvailableSelections
    {
        /// <summary>
        /// Gets the <see cref="string"/> of the event
        /// </summary>
        string Event { get; }

        /// <summary>
        /// Gets the list of markets for this event
        /// </summary>
        IEnumerable<IMarket> Markets { get; }
    }
}
