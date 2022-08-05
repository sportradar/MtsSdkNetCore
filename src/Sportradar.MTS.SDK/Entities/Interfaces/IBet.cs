/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for a ticket bet
    /// </summary>
    public interface IBet
    {
        /// <summary>
        /// Gets the bonus of the bet (optional, default null)
        /// </summary>
        IBetBonus Bonus { get; }

        /// <summary>
        /// Gets the free stkae of the bet (optional, default null)
        /// </summary>
        /// <value>The free stake</value>
        IFreeStake FreeStake { get; }

        /// <summary>
        /// Gets the stake of the bet
        /// </summary>
        IStake Stake { get; }

        /// <summary>
        /// Gets the entire stake of the bet
        /// </summary>
        IStake EntireStake { get; }

        /// <summary>
        /// Gets the id of the bet
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets array of all the systems (optional, if missing then complete accumulator is used)
        /// </summary>
        IEnumerable<int> SelectedSystems { get; }

        /// <summary>
        /// Gets the array of selections which form the bet
        /// </summary>
        IEnumerable<ISelection> Selections { get; }

        /// <summary>
        /// Gets the reoffer reference bet id
        /// </summary>
        string ReofferRefId { get; }

        /// <summary>
        /// Gets the sum of all wins for all generated combinations for this bet (in ticket currency, used in validation)
        /// </summary>
        long SumOfWins { get; }

        /// <summary>
        /// Gets the flag if bet is a custom bet (optional, default false)
        /// </summary>
        bool? CustomBet { get; }

        /// <summary>
        /// Gets the odds calculated for custom bet multiplied by 10_000 and rounded to int value
        /// </summary>
        int? CalculationOdds { get; }
    }
}