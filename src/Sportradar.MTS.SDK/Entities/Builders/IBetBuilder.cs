/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes implementing builder for <see cref="IBet"/>
    /// </summary>
    public interface IBetBuilder
    {
        /// <summary>
        /// Sets the <see cref="IBetBonus" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="betBonusMode">The bet bonus mode</param>
        /// <param name="betBonusType">Type of the bet bonus</param>
        /// <param name="description">Description of the bet bonus</param>
        /// <param name="paidAs">PaidAs type of the bet bonus</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetBetBonus(long value, BetBonusMode betBonusMode = BetBonusMode.All, BetBonusType betBonusType = BetBonusType.Total,
            BetBonusDescription? description = null, BetBonusPaidAs? paidAs = null);

        /// <summary>
        /// Sets the <see cref="IFreeStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="freeStakeType">Type of the free stake</param>
        /// <param name="description">Description of the free stake</param>
        /// <param name="paidAs">PaidAs type of the free stake</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetFreeStake(long value, FreeStakeType? freeStakeType = null, FreeStakeDescription? description = null, FreeStakePaidAs? paidAs = null);

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetStake(long value);

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="stakeType">Type of the stake</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetStake(long value, StakeType stakeType);

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetEntireStake(long value);

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="stakeType">Type of the stake</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetEntireStake(long value, StakeType stakeType);

        /// <summary>
        /// Sets the bet id
        /// </summary>
        /// <param name="id">The  bet id</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetBetId(string id);

        /// <summary>
        /// Add system to <see cref="IBet.SelectedSystems" />
        /// </summary>
        /// <param name="systemId">The system id</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder AddSelectedSystem(int systemId);

        /// <summary>
        /// Gets the array of selected systems
        /// </summary>
        /// <value>The array of selected systems</value>
        IEnumerable<int> GetSelectedSystems { get; }

        /// <summary>
        /// Sets the reoffer reference bet id
        /// </summary>
        /// <param name="reofferRefId">The reoffer reference id</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetReofferRefId(string reofferRefId);

        /// <summary>
        /// Sets the sum of all wins for all generated combinations for this bet (in ticket currency, used for validation)
        /// </summary>
        /// <param name="sum">The sum to be set</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder SetSumOfWins(long sum);

        /// <summary>
        /// Adds the selection
        /// </summary>
        /// <param name="selection">A <see cref="ISelection"/> to be added to this bet</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        IBetBuilder AddSelection(ISelection selection);

        /// <summary>
        /// Gets the selections
        /// </summary>
        /// <returns>Returns all the selections</returns>
        IEnumerable<ISelection> GetSelections();

        /// <summary>
        /// Sets the flag if bet is a custom bet (optional, default false)
        /// </summary>
        /// <param name="customBet">The flag if bet is a custom bet (optional, default false)</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        IBetBuilder SetCustomBet(bool? customBet);

        /// <summary>
        /// Sets the odds calculated for custom bet multiplied by 10_000 and rounded to int value
        /// </summary>
        /// <param name="calculationOdds">The odds calculated for custom bet multiplied by 10_000 and rounded to int value</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        IBetBuilder SetCalculationOdds(int? calculationOdds);
        
        /// <summary>
        /// Builds the <see cref="IBet" />
        /// </summary>
        /// <returns>Returns a <see cref="IBet"/></returns>
        IBet Build();
    }
}
