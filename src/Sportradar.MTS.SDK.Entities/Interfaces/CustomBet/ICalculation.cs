/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces.CustomBet
{
    /// <summary>
    /// Provides a probability calculation
    /// </summary>
    public interface ICalculation
    {
        /// <summary>
        /// Gets the odds
        /// </summary>
        double Odds { get; }

        /// <summary>
        /// Gets the probability
        /// </summary>
        double Probability { get; }
    }
}
