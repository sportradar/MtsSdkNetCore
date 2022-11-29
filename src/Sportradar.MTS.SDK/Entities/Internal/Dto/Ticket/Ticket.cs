/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    /// <summary>
    /// Class Ticket
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    internal partial class Ticket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ticket"/> class
        /// </summary>
        public Ticket()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ticket"/> class
        /// </summary>
        /// <param name="ticketId">The ticket identifier</param>
        /// <param name="sender">The sender</param>
        /// <param name="bets">The bets</param>
        /// <param name="selections">The selections</param>
        /// <param name="oddsChange">The odds change</param>
        /// <param name="isTestSource">if set to <c>true</c> [is test source]</param>
        /// <param name="timestamp">The timestamp</param>
        /// <param name="version">The version</param>
        /// <param name="reofferRefId">The reoffer reference id</param>
        /// <param name="altStakeRefId">The alternative stake reference id</param>
        /// <param name="totalCombinations">Expected total number of generated combinations on this ticket (optional, default null). If present is used to validate against actual number of generated combinations</param>
        /// <param name="lastMatchEndTime">Time of last non-sportradar selection on ticket</param>
        /// <param name="payCap">Capped max payout of the ticket</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        public Ticket(string ticketId, 
                      Sender sender, 
                      IEnumerable<Anonymous> bets, 
                      IEnumerable<Anonymous2> selections, 
                      TicketOddsChange oddsChange, 
                      bool isTestSource, 
                      DateTime timestamp, 
                      string version, 
                      string reofferRefId, 
                      string altStakeRefId, 
                      int? totalCombinations, 
                      DateTime? lastMatchEndTime,
                      long? payCap)
        {
            TicketId = ticketId;
            Sender = sender;
            Bets = bets as ObservableCollection<Anonymous>;
            Selections = selections as ObservableCollection<Anonymous2>;
            OddsChange = oddsChange;
            TestSource = isTestSource;
            TimestampUtc = MtsTicketHelper.Convert(timestamp);
            Version = version;
            ReofferRefId = string.IsNullOrEmpty(reofferRefId) ? null : reofferRefId;
            AltStakeRefId = string.IsNullOrEmpty(altStakeRefId) ? null : altStakeRefId;
            TotalCombinations = totalCombinations;
            LastMatchEndTime = lastMatchEndTime == null ? (long?)null : MtsTicketHelper.Convert(lastMatchEndTime.Value);
            PayCap = payCap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ticket"/> class
        /// </summary>
        /// <param name="ticket">The ticket</param>
        public Ticket(ITicket ticket)
        {
            TicketId = ticket.TicketId;
            Sender = new Sender(ticket.Sender);
            OddsChange = ticket.OddsChange.HasValue ? MtsTicketHelper.Convert(ticket.OddsChange.Value) : (TicketOddsChange?)null;
            TestSource = ticket.TestSource;
            TimestampUtc = MtsTicketHelper.Convert(ticket.Timestamp);
            Version = ticket.Version;
            var selections = ticket.Selections.ToList().ConvertAll(c => new Anonymous2(c));
            Selections = selections.Distinct().ToList();
            Bets = ticket.Bets.ToList().ConvertAll(b => new Anonymous(b, GetBetSelectionRefs(b, Selections as IReadOnlyList<Anonymous2>, ticket.Selections.Any(a=>a.IsBanker))));
            ReofferRefId = string.IsNullOrEmpty(ticket.ReofferId) ? null : ticket.ReofferId;
            AltStakeRefId = string.IsNullOrEmpty(ticket.AltStakeRefId) ? null : ticket.AltStakeRefId;
            TotalCombinations = ticket.TotalCombinations;
            LastMatchEndTime = ticket.LastMatchEndTime == null ? (long?) null : MtsTicketHelper.Convert(ticket.LastMatchEndTime.Value);
            PayCap = ticket.PayCap;
        }

        /// <summary>
        /// Gets the bet selection refs
        /// </summary>
        /// <param name="bet">The bet</param>
        /// <param name="allSelections">All selections</param>
        /// <param name="hasBanker">There is a banker in any of the selection</param>
        /// <returns>IEnumerable&lt;ISelectionRef&gt;</returns>
        private static IEnumerable<ISelectionRef> GetBetSelectionRefs(IBet bet, IReadOnlyList<Anonymous2> allSelections, bool hasBanker)
        {
            if (bet.Selections.Count() != allSelections.Count || bet.Selections.Any(s => s.IsBanker) || hasBanker)
            {
                var refs = new List<ISelectionRef>();
                foreach (var betSelection in bet.Selections)
                {
                    refs.Add(new SelectionRef(FindSelectionIndex(allSelections, betSelection), betSelection.IsBanker));
                }
                return refs;
            }
            return new List<ISelectionRef>();
        }

        /// <summary>
        /// Finds the index of the selection
        /// </summary>
        /// <param name="allSelections">All selections</param>
        /// <param name="specific">The specific</param>
        /// <returns>The index of the selection</returns>
        private static int FindSelectionIndex(IReadOnlyList<Anonymous2> allSelections, ISelection specific)
        {
            for (var i = 0; i < allSelections.Count; i++)
            {
                var sel = allSelections[i];
                if (sel.EventId == specific.EventId && sel.Id == specific.Id && sel.Odds == specific.Odds)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}