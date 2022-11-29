/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="IBet"/>
    /// </summary>
    /// <seealso cref="IBet" />
    internal class Bet : IBet
    {
        /// <summary>
        /// Gets the bonus of the bet (optional, default null)
        /// </summary>
        /// <value>The bonus</value>
        public IBetBonus Bonus { get; }

        /// <summary>
        /// Gets the free stkae of the bet (optional, default null)
        /// </summary>
        /// <value>The free stake</value>
        public IFreeStake FreeStake { get; }

        /// <summary>
        /// Gets the stake of the bet
        /// </summary>
        /// <value>The stake</value>
        public IStake Stake { get; }
        /// <summary>
        /// Gets the entire stake of the bet
        /// </summary>
        public IStake EntireStake { get; }
        /// <summary>
        /// Gets the id of the bet
        /// </summary>
        /// <value>The identifier</value>
        public string Id { get; }
        /// <summary>
        /// Gets array of all the systems (optional, if missing then complete accumulator is used)
        /// </summary>
        /// <value>The selected systems</value>
        public IEnumerable<int> SelectedSystems { get; }
        /// <summary>
        /// Gets the array of selection references which form the bet (optional, if missing then all selections are used)
        /// </summary>
        /// <value>The selection refs</value>
        public IEnumerable<ISelection> Selections { get; }
        /// <summary>
        /// Gets the reoffer reference bet id
        /// </summary>
        /// <value>The reoffer reference identifier</value>
        public string ReofferRefId { get; }
        /// <summary>
        /// Gets the sum of all wins for all generated combinations for this bet (in ticket currency, used in validation)
        /// </summary>
        /// <value>The sum of wins</value>
        public long SumOfWins { get; }
        /// <summary>
        /// Gets the flag if bet is a custom bet (optional, default false)
        /// </summary>
        public bool? CustomBet { get; }
        /// <summary>
        /// Gets the odds calculated for custom bet multiplied by 10_000 and rounded to int value
        /// </summary>
        public int? CalculationOdds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bet"/> class
        /// </summary>
        /// <param name="bonus">The bonus</param>
        /// <param name="stake">The stake</param>
        /// <param name="freeStake">The free stake</param>
        /// <param name="entireStake">The entire stake</param>
        /// <param name="id">The bet identifier</param>
        /// <param name="selectedSystems">The selected systems</param>
        /// <param name="selections">The selections</param>
        /// <param name="reofferRefId">The reoffer reference identifier</param>
        /// <param name="sumOfWins">The sum of wins</param>
        /// <param name="customBet">The flag if bet is a custom bet</param>
        /// <param name="calculationOdds">The odds calculated for custom bet</param>
        [JsonConstructor]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        public Bet(IBetBonus bonus, 
                   IStake stake,
                   IFreeStake freeStake,
                   IStake entireStake, 
                   string id, 
                   IEnumerable<int> selectedSystems, 
                   IEnumerable<ISelection> selections, 
                   string reofferRefId, 
                   long sumOfWins, 
                   bool? customBet, 
                   int? calculationOdds)
        {
            Guard.Argument(stake, nameof(stake)).NotNull();
            Guard.Argument(id, nameof(id)).Require(string.IsNullOrEmpty(id) || TicketHelper.ValidateTicketId(id));
            var systems = selectedSystems == null ? new List<int>() : selectedSystems.ToList();
            Guard.Argument(systems, nameof(systems)).Require(selectedSystems == null
                              || (systems.Any()
                              //&& systems.Count < 64
                              && systems.Count == systems.Distinct().Count()
                              && systems.All(a => a > 0)));
            var listSelections = selections.ToList();
            Guard.Argument(listSelections, nameof(listSelections)).NotNull().NotEmpty().Require(/*listSelections.Count < 64 && */listSelections.Count == listSelections.Distinct().Count());
            Guard.Argument(reofferRefId, nameof(reofferRefId)).Require(string.IsNullOrEmpty(reofferRefId) || reofferRefId.Length <= 50);
            Guard.Argument(sumOfWins, nameof(sumOfWins)).NotNegative();
            bool customBetBool = customBet ?? false;
            Guard.Argument(customBet, nameof(customBet)).Require((customBetBool && calculationOdds != null && calculationOdds >= 0) || (!customBetBool && calculationOdds == null));

            Bonus = bonus;
            Stake = stake;
            FreeStake = freeStake;
            EntireStake = entireStake;
            Id = id;
            SelectedSystems = systems;
            Selections = listSelections;
            ReofferRefId = reofferRefId;
            SumOfWins = sumOfWins;
            CustomBet = customBet;
            CalculationOdds = calculationOdds;

            if (SelectedSystems != null)
            {
                var enumerable = SelectedSystems as IList<int> ?? SelectedSystems.ToList();
                if (SelectedSystems != null && enumerable.Any(a => a > Selections.Count()))
                {
                    throw new ArgumentException("Invalid value in SelectedSystems.");
                }
            }
        }
    }
}