/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for a customer confidence factor
    /// </summary>
    public interface ICcf
    {
        /// <summary>
        /// Gets the customer confidence factor (factor multiplied by 10000)
        /// </summary>
        long Ccf { get; }

        /// <summary>
        /// Gets <see cref="ISportCcf"/> values for sport and prematch/live (if set for customer)
        /// </summary>
        IEnumerable<ISportCcf> SportCcfDetails { get; }
    }
}