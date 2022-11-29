/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dawn;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="IBetBuilder"/>
    /// </summary>
    /// <seealso cref="IBetBuilder" />
    internal class BetBuilder : IBetBuilder
    {
        /// <summary>
        /// The reoffer reference identifier
        /// </summary>
        private string _reofferRefId;

        /// <summary>
        /// The bet identifier
        /// </summary>
        private string _betId;

        /// <summary>
        /// The sum
        /// </summary>
        private long _sum;

        /// <summary>
        /// The selected systems
        /// </summary>
        private IEnumerable<int> _selectedSystems;

        /// <summary>
        /// The bet bonus
        /// </summary>
        private IBetBonus _betBonus;


        /// <summary>
        /// The free stkae
        /// </summary>
        private IFreeStake _freeStake;

        /// <summary>
        /// The stake
        /// </summary>
        private IStake _stake;

        /// <summary>
        /// The stake
        /// </summary>
        private IStake _entireStake;

        /// <summary>
        /// The list of all selections for this bet
        /// </summary>
        private List<ISelection> _selections;

        /// <summary>
        /// The flag if bet is a custom bet (optional, default false)
        /// </summary>
        private bool? _customBet;

        /// <summary>
        /// The odds calculated for custom bet multiplied by 10_000 and rounded to int value
        /// </summary>
        private int? _calculationOdds;

        /// <summary>
        /// Gets the array of selected systems
        /// </summary>
        /// <value>The array of selected systems</value>
        public IEnumerable<int> GetSelectedSystems => _selectedSystems;

        /// <summary>
        /// Sets the bet id
        /// </summary>
        /// <param name="id">The  bet id</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetBetId(string id)
        {
            _betId = id;
            ValidateData(false, true);
            return this;
        }

        /// <summary>
        /// Add system to <see cref="IBet.SelectedSystems" />
        /// </summary>
        /// <param name="systemId">The system id</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder AddSelectedSystem(int systemId)
        {
            var selectedSystems = _selectedSystems as List<int> ?? new List<int>();
            if (!selectedSystems.Contains(systemId))
            {
                selectedSystems.Add(systemId);
                _selectedSystems = selectedSystems;
            }
            ValidateData(false, false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the reoffer reference bet id
        /// </summary>
        /// <param name="reofferRefId">The reoffer reference id</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetReofferRefId(string reofferRefId)
        {
            _reofferRefId = reofferRefId;
            ValidateData(false, false, false, false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the sum of all wins for all generated combinations for this bet (in ticket currency, used for validation)
        /// </summary>
        /// <param name="sum">The sum to be set</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetSumOfWins(long sum)
        {
            if (sum <= 0)
            {
                throw new ArgumentException("SumOfWins value not valid.");
            }
            _sum = sum;
            ValidateData(false, false, false, false, false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IBetBonus" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="betBonusMode">The bet bonus mode</param>
        /// <param name="betBonusType">Type of the bet bonus</param>
        /// <param name="description">Description of the bet bonus</param>
        /// <param name="paidAs">PaidAs type of the bet bonus</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetBetBonus(long value, BetBonusMode betBonusMode = BetBonusMode.All, BetBonusType betBonusType = BetBonusType.Total,
            BetBonusDescription? description = null, BetBonusPaidAs? paidAs = null)
        {
            if (!(value > 0 && value < 1000000000000000000))
            {
                throw new ArgumentException("BetBonus value not valid. Must be greater then zero.");
            }
            _betBonus = new BetBonus(value, betBonusType, betBonusMode, description, paidAs);
            return this;
        }


        /// <summary>
        /// Sets the <see cref="IFreeStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="freeStakeType">Type of the free stake</param>
        /// <param name="description">Description of the free stake</param>
        /// <param name="paidAs">PaidAs type of the free stake</param>
        /// <returns>Returns a <see cref="IBetBuilder"/></returns>
        public IBetBuilder SetFreeStake(long value, FreeStakeType? freeStakeType = null, FreeStakeDescription? description = null, FreeStakePaidAs? paidAs = null)
        {
            _freeStake = new FreeStake(value, freeStakeType, description, paidAs);
            ValidateData(false, false, false, false, false, false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetStake(long value)
        {
            _stake = new Stake(value);
            ValidateData(false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="stakeType">Type of the stake</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetStake(long value, StakeType stakeType)
        {
            _stake = new Stake(value, stakeType);
            ValidateData(false, false, true);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetEntireStake(long value)
        {
            _entireStake = new Stake(value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IStake" />
        /// </summary>
        /// <param name="value">The quantity multiplied by 10000 and rounded to a long value</param>
        /// <param name="stakeType">Type of the stake</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetEntireStake(long value, StakeType stakeType)
        {
            _entireStake = new Stake(value, stakeType);
            return this;
        }

        /// <summary>
        /// Adds the selection
        /// </summary>
        /// <param name="selection">A <see cref="ISelection" /> to be added to this bet</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IBetBuilder AddSelection(ISelection selection)
        {
            var selections = _selections ?? new List<ISelection>();
            var similarSel = selections.Find(f => f.EventId == selection.EventId && f.Id == selection.Id);
            if (similarSel != null)
            {
                if (similarSel.Odds == selection.Odds && similarSel.IsBanker == selection.IsBanker)
                {
                    return this;
                }
                throw new ArgumentException("Bet can not have selections with the same eventId, id and different odds or different banker value.");
            }
            selections.Add(selection);
            _selections = selections;
            ValidateData(false, false, false, false, true);
            return this;
        }

        /// <summary>
        /// Gets the selections
        /// </summary>
        /// <returns>Returns all the selections</returns>
        public IEnumerable<ISelection> GetSelections()
        {
            return _selections;
        }

        /// <summary>
        /// Sets the flag if bet is a custom bet (optional, default false)
        /// </summary>
        /// <param name="customBet">The flag if bet is a custom bet (optional, default false)</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetCustomBet(bool? customBet)
        {
            _customBet = customBet;
            return this;
        }

        /// <summary>
        /// Sets the odds calculated for custom bet multiplied by 10_000 and rounded to int value
        /// </summary>
        /// <param name="calculationOdds">The odds calculated for custom bet multiplied by 10_000 and rounded to int value</param>
        /// <returns>Returns a <see cref="IBetBuilder" /></returns>
        public IBetBuilder SetCalculationOdds(int? calculationOdds)
        {
            _calculationOdds = calculationOdds;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="IBet" />
        /// </summary>
        /// <returns>Returns a <see cref="IBet" /></returns>
        public IBet Build()
        {
            ValidateData(true);
            return new Bet(_betBonus, _stake, _freeStake, _entireStake, _betId, _selectedSystems, _selections, _reofferRefId, _sum, _customBet, _calculationOdds);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Approved")]
        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        private void ValidateData(bool all = false, bool betId = false, bool stake = false, bool selectedSystems = false, bool selections = false, bool reofferRefId = false, bool sumOfWins = false, bool freeStake = false)
        {
            if ((all || betId) && !string.IsNullOrEmpty(_betId) && !TicketHelper.ValidateTicketId(_betId))
            {
                throw new ArgumentException("BetId not valid.");
            }
            if ((all || stake) && _stake == null)
            {
                throw new ArgumentException("Stake not valid.");
            }
            if ((all || freeStake) && _freeStake == null && _stake != null)
            {
                //check for stake if value is zero when there is no free stake!!!
                Guard.Argument(_stake.Value, nameof(_stake.Value)).InRange(1, 1000000000000000000);
            }
            if ((all || selectedSystems) && !(_selectedSystems == null
                                              || (_selectedSystems.Any()
                                                  //&& _selectedSystems.Count() < 64
                                                  && _selectedSystems.Count() == _selectedSystems.Distinct().Count()
                                                  && _selectedSystems.All(a => a > 0))))
            {
                throw new ArgumentException("SelectedSystems not valid.");
            }
            if ((all || selections) && !(_selections != null
                                         && _selections.Any()
                                         //&& _selections.Count < 64
                                         && _selections.Count == _selections.Distinct().Count()))
            {
                throw new ArgumentException("Selections not valid.");
            }
            if ((all || reofferRefId) && !(string.IsNullOrEmpty(_reofferRefId) || _reofferRefId.Length <= 50))
            {
                throw new ArgumentException("ReofferRefId not valid.");
            }
            if ((all || sumOfWins) && _sum < 0)
            {
                throw new ArgumentException("SumOfWins not valid.");
            }

            if (all && _selectedSystems != null && (_selectedSystems.Count() > _selections.Count || _selectedSystems.Any(a => a > _selections.Count)))
            {
                throw new ArgumentException("SelectionSystem are not valid.");
            }
        }
    }
}