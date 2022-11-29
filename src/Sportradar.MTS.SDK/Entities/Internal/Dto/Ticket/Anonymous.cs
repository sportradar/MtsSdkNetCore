/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    /// <summary>
    /// Class for BET
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    internal partial class Anonymous
    {
        public Anonymous()
        { }

        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        public Anonymous(string id, long sumOfWins, Stake stake, Bonus bonus, FreeStake freeStake, IEnumerable<int> selectedSystems, IEnumerable<Anonymous3> selectionRefs, string reofferRefId)
        {
            _id = string.IsNullOrEmpty(id) ? null : id;
            _sumOfWins = sumOfWins > 0 ? sumOfWins : (long?)null;
            _stake = stake;
            _bonus = null;
            if (bonus != null)
            {
                _bonus = bonus;
            }
            _freeStake = null;
            if (freeStake != null)
            {
                _freeStake = freeStake;
            }

            _selectedSystems = null;
            if (selectedSystems != null && selectedSystems.Any())
            {
                _selectedSystems = selectedSystems as IReadOnlyCollection<int>;
            }
            _selectionRefs = null;
            if (selectionRefs != null && selectionRefs.Any())
            {
                _selectionRefs = selectionRefs as IReadOnlyCollection<Anonymous3>;
            }
            _reofferRefId = string.IsNullOrEmpty(reofferRefId) ? null : reofferRefId;

            _customBet = null;
            _calculationOdds = null;
            _entireStake = null;
        }

        public Anonymous(IBet bet, IEnumerable<ISelectionRef> selectionRefs)
        {
            _id = string.IsNullOrEmpty(bet.Id) ? null : bet.Id;
            _sumOfWins = bet.SumOfWins > 0 ? bet.SumOfWins : (long?)null;
            _stake = new Stake(bet.Stake);
            _entireStake = bet.EntireStake != null ? new EntireStake(bet.EntireStake) : null;
            _bonus = null;
            if (bet.Bonus != null)
            {
                _bonus = new Bonus(bet.Bonus);
            }
            _freeStake = null;
            if (bet.FreeStake != null)
            {
                _freeStake = new FreeStake(bet.FreeStake);
            }

            _selectedSystems = null;
            if (bet.SelectedSystems != null && bet.SelectedSystems.Any())
            {
                _selectedSystems = bet.SelectedSystems as IReadOnlyCollection<int>;
            }
            _selectionRefs = null;
            var selectionRefsList = selectionRefs.ToList();
            if (selectionRefsList.Any())
            {
                _selectionRefs = selectionRefsList.ConvertAll(b => new Anonymous3(b));
            }

            _customBet = bet.CustomBet;
            _calculationOdds = bet.CalculationOdds;
        }
    }
}