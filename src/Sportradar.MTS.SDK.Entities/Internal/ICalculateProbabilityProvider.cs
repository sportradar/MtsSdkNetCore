/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportradar.MTS.SDK.Entities.Interfaces.CustomBet;
using Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Defines a contract implemented by classes used to provide data probability calculations
    /// </summary>
    public interface ICalculateProbabilityProvider
    {
        /// <summary>
        /// Asynchronously gets a <see cref="CalculationDTO"/> instance
        /// </summary>
        /// <param name="selections">The <see cref="IEnumerable{ISelection}"/> containing selections for which the probability should be fetched</param>
        /// <returns>A <see cref="Task{CalculationDTO}"/> representing the probability calculation</returns>
        Task<CalculationDTO> GetDataAsync(IEnumerable<ISelection> selections);
    }
}