/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="ITicketBuilder" />
    /// </summary>
    /// <seealso cref="ITicketBuilder" />
    internal class TicketBuilder : ITicketBuilder
    {
        /// <summary>
        /// The ticket identifier
        /// </summary>
        private string _ticketId;

        /// <summary>
        /// The reoffer identifier
        /// </summary>
        private string _reofferId;

        /// <summary>
        /// The alt stake reference identifier
        /// </summary>
        private string _altStakeRefId;

        /// <summary>
        /// The is test
        /// </summary>
        private bool _isTest;

        /// <summary>
        /// The odds change type
        /// </summary>
        private OddsChangeType? _oddsChangeType;

        /// <summary>
        /// The sender
        /// </summary>
        private ISender _sender;

        /// <summary>
        /// The bets
        /// </summary>
        private IEnumerable<IBet> _bets;

        /// <summary>
        /// The total combinations
        /// </summary>
        private int? _totalCombinations;

        /// <summary>
        /// End time of last (non Sportradar) match on ticket
        /// </summary>
        private DateTime? _lastMatchEndTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketBuilder"/> class
        /// </summary>
        internal TicketBuilder()
        {
            _totalCombinations = null;
        }

        /// <summary>
        /// Sets the ticket id
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder SetTicketId(string ticketId)
        {
            if (!TicketHelper.ValidateTicketId(ticketId))
            {
                throw new ArgumentException("TicketId not valid");
            }
            _ticketId = ticketId;
            return this;
        }

        /// <summary>
        /// Sets the reoffer id
        /// </summary>
        /// <param name="reofferId">The reoffer id</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder SetReofferId(string reofferId)
        {
            if (!TicketHelper.ValidateTicketId(reofferId))
            {
                throw new ArgumentException("ReofferId not valid");
            }
            _reofferId = reofferId;
            return this;
        }

        /// <summary>
        /// Sets the alternative stake reference ticket id
        /// </summary>
        /// <param name="altStakeRefId">The alt stake reference id</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder SetAltStakeRefId(string altStakeRefId)
        {
            if (!TicketHelper.ValidateTicketId(altStakeRefId))
            {
                throw new ArgumentException("AltStakeRefId not valid");
            }
            _altStakeRefId = altStakeRefId;
            return this;
        }

        /// <summary>
        /// Sets the test source
        /// </summary>
        /// <param name="isTest">if set to <c>true</c> [is test]</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder SetTestSource(bool isTest)
        {
            _isTest = isTest;
            return this;
        }

        /// <summary>
        /// Sets the odds change
        /// </summary>
        /// <param name="type">The <see cref="OddsChangeType" /> to be set</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder SetOddsChange(OddsChangeType type)
        {
            _oddsChangeType = type;
            return this;
        }

        /// <summary>
        /// Sets the sender
        /// </summary>
        /// <param name="sender">The ticket sender</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder SetSender(ISender sender)
        {
            _sender = sender ?? throw new ArgumentException("Sender not valid");
            return this;
        }

        /// <summary>
        /// Sets the expected total number of generated combinations on this ticket (optional, default null). If present, it is used to validate against actual number of generated combinations.
        /// </summary>
        /// <param name="totalCombinations">The expected total number of generated combinations</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        public ITicketBuilder SetTotalCombinations(int totalCombinations)
        {
            if (totalCombinations < 1)
            {
                throw new ArgumentException("totalCombinations must be greater then zero");
            }
            _totalCombinations = totalCombinations;
            return this;
        }

        /// <summary>
        /// End time of last (non Sportradar) match on ticket
        /// </summary>
        /// <param name="lastMatchEndTime">End time of last (non Sportradar) match on ticket</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder SetLastMatchEndTime(DateTime lastMatchEndTime)
        {
            if (lastMatchEndTime < DateTime.Now)
            {
                throw new ArgumentException("LastMatchEndTime not valid or in the past.");
            }
            _lastMatchEndTime = lastMatchEndTime;
            return this;
        }

        /// <summary>
        /// Adds the <see cref="IBet" />
        /// </summary>
        /// <param name="bet">A <see cref="IBet" /> to be added to this ticket</param>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ITicketBuilder AddBet(IBet bet)
        {
            var bets = _bets as List<IBet> ?? new List<IBet>();
            bets.Add(bet);
            _bets = bets;
            return this;
        }

        /// <summary>
        /// Gets the bets
        /// </summary>
        /// <returns>Returns all the bets</returns>
        public IEnumerable<IBet> GetBets() => _bets;

        /// <summary>
        /// Builds the <see cref="ITicket" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicket" /></returns>
        public ITicket BuildTicket()
        {
            var ticket = new Ticket(_ticketId, _sender, _bets, _reofferId, _altStakeRefId, _isTest, _oddsChangeType, _totalCombinations, _lastMatchEndTime);
            _ticketId = null;
            _sender = null;
            _bets = null;
            _reofferId = null;
            _altStakeRefId = null;
            _isTest = false;
            _oddsChangeType = null;
            _totalCombinations = null;
            _lastMatchEndTime = null;
            return ticket;
        }
    }
}