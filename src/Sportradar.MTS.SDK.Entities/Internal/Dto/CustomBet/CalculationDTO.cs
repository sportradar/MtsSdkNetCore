/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Sportradar.MTS.SDK.Entities.Internal.REST;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet
{
    /// <summary>
    /// Defines a data-transfer-object for probability calculations
    /// </summary>
    public class CalculationDTO
    {
        /// <summary>
        /// Gets the odds
        /// </summary>
        public double Odds { get; }

        /// <summary>
        /// Gets the probability
        /// </summary>
        public double Probability { get; }

        internal CalculationDTO(CalculationResponseType calculation)
        {
            if (calculation == null)
                throw new ArgumentNullException(nameof(calculation));

            Odds = calculation.calculation.odds;
            Probability = calculation.calculation.probability;
        }
    }
}
