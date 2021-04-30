/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="ITicketAltStakeBuilder"/>
    /// </summary>
    /// <seealso cref="ITicketAltStakeBuilder" />
    internal class TicketAltStakeBuilder : ITicketAltStakeBuilder
    {
        private readonly ISimpleBuilderFactory _builderFactory;
        private ITicket _ticket;
        private ITicketResponse _ticketResponse;
        private string _newTicketId;
        private long _newStake;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketAltStakeBuilder"/> class
        /// </summary>
        /// <param name="builderFactory">A <see cref="SimpleBuilderFactory"/> used to construct entity builders</param>
        internal TicketAltStakeBuilder(ISimpleBuilderFactory builderFactory)
        {
            Guard.Argument(builderFactory, nameof(builderFactory)).NotNull();

            _builderFactory = builderFactory;
        }

        /// <summary>
        /// Sets the original ticket and the ticket response
        /// </summary>
        /// <param name="ticket">The original ticket</param>
        /// <param name="ticketResponse">The ticket response from which the stake info will be used</param>
        /// <param name="newTicketId">The new alternative stake ticket id</param>
        /// <returns>Returns the <see cref="ITicketAltStakeBuilder" /></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        public ITicketAltStakeBuilder Set(ITicket ticket, ITicketResponse ticketResponse, string newTicketId = null)
        {
            _ticket = ticket;
            _ticketResponse = ticketResponse;
            _newTicketId = newTicketId;
            return this;
        }

        /// <summary>
        /// Sets the original ticket and the ticket response
        /// </summary>
        /// <param name="ticket">The original ticket</param>
        /// <param name="newStake">The new stake value which will be used to set bet stake</param>
        /// <param name="newTicketId">The new alternative stake ticket id</param>
        /// <returns>Returns the <see cref="ITicketAltStakeBuilder" /></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        public ITicketAltStakeBuilder Set(ITicket ticket, long newStake, string newTicketId = null)
        {
            _ticket = ticket;
            _newStake = newStake;
            _newTicketId = newTicketId;
            return this;
        }

        /// <summary>
        /// Builds the new <see cref="ITicket" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicket" /></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ITicket BuildTicket()
        {
            return _ticketResponse != null
                ? BuildAltStakeTicket(_builderFactory, _ticket, _ticketResponse, _newTicketId)
                : BuildAltStakeTicket(_builderFactory, _ticket, _newStake, _newTicketId);
        }

        /// <summary>
        /// Builds the alternative stake ticket based on the original ticket and the ticket response
        /// </summary>
        /// <param name="builderFactory">A <see cref="SimpleBuilderFactory"/> used to construct entity builders</param>
        /// <param name="orgTicket">The original ticket</param>
        /// <param name="ticketResponse">The ticket response from which the stake info will be used</param>
        /// <param name="newTicketId">The new alternative ticket id</param>
        /// <returns>Returns the <see cref="ITicket"/> representing the alternative ticket</returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        /// <exception cref="ArgumentException">Only tickets with exactly 1 bet are supported</exception>
        private static ITicket BuildAltStakeTicket(ISimpleBuilderFactory builderFactory, ITicket orgTicket, ITicketResponse ticketResponse, string newTicketId = null)
        {
            if (orgTicket.Bets.Count() != 1)
            {
                throw new ArgumentException("Only tickets with exactly 1 bet are supported.");
            }
            if (ticketResponse.BetDetails.Any(a => a.AlternativeStake.Stake == 0))
            {
                throw new ArgumentException("Response bet details are missing alternative stake info.");
            }

            if (orgTicket.Bets.Count() == 1)
            {
                return BuildAltStakeTicket(builderFactory, orgTicket, ticketResponse.BetDetails.First().AlternativeStake.Stake, newTicketId);
            }

            var altStakeTicketBuilder = builderFactory.CreateTicketBuilder()
                .SetTicketId(string.IsNullOrEmpty(newTicketId) ? orgTicket.TicketId + "A" : newTicketId)
                .SetSender(orgTicket.Sender)
                .SetTestSource(orgTicket.TestSource)
                .SetAltStakeRefId(orgTicket.TicketId);

            if (orgTicket.OddsChange.HasValue)
            {
                altStakeTicketBuilder.SetOddsChange(orgTicket.OddsChange.Value);
            }

            foreach (var ticketBet in orgTicket.Bets)
            {
                var responseBetDetail = ticketResponse.BetDetails.First(f => f.BetId == ticketBet.Id);
                if (responseBetDetail == null)
                {
                    throw new ArgumentException($"Ticket response is missing a bet details for the bet {ticketBet.Id}");
                }
                var newBetBuilder = builderFactory.CreateBetBuilder()
                    .SetBetId(ticketBet.Id + "A")
                    .SetReofferRefId(ticketBet.Id)
                    .SetSumOfWins(ticketBet.SumOfWins);

                altStakeTicketBuilder.AddBet(BuilderHelper.BuildBetFromExisting(newBetBuilder, ticketBet, responseBetDetail.AlternativeStake.Stake));
            }
            return altStakeTicketBuilder.BuildTicket();
        }

        /// <summary>
        /// Builds the alternative stake ticket based on the original ticket and the ticket response
        /// </summary>
        /// <param name="builderFactory">A <see cref="SimpleBuilderFactory"/> used to construct entity builders</param>
        /// <param name="orgTicket">The original ticket</param>
        /// <param name="newStake">The new stake value which will be used to set bet stake</param>
        /// <param name="newTicketId">The new alternative ticket id</param>
        /// <returns>Returns the <see cref="ITicket"/> representing the alternative ticket</returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        /// <exception cref="ArgumentException">Only tickets with exactly 1 bet are supported</exception>
        private static ITicket BuildAltStakeTicket(ISimpleBuilderFactory builderFactory, ITicket orgTicket, long newStake, string newTicketId = null)
        {
            if (orgTicket.Bets.Count() != 1)
            {
                throw new ArgumentException("Only tickets with exactly 1 bet are supported.");
            }
            if (newStake <= 0)
            {
                throw new ArgumentException("New stake info is invalid.");
            }

            var altStakeTicketBuilder = builderFactory.CreateTicketBuilder()
                                         .SetTicketId(string.IsNullOrEmpty(newTicketId) ? orgTicket.TicketId + "A" : newTicketId)
                                         .SetSender(orgTicket.Sender)
                                         .SetTestSource(orgTicket.TestSource)
                                         .SetAltStakeRefId(orgTicket.TicketId);

            if (orgTicket.OddsChange.HasValue)
            {
                altStakeTicketBuilder.SetOddsChange(orgTicket.OddsChange.Value);
            }

            foreach (var ticketBet in orgTicket.Bets)
            {
                var newBetBuilder = builderFactory.CreateBetBuilder()
                    .SetBetId(ticketBet.Id + "A")
                    .SetReofferRefId(ticketBet.Id);
                
                altStakeTicketBuilder.AddBet(BuilderHelper.BuildBetFromExisting(newBetBuilder, ticketBet, newStake));
            }
            return altStakeTicketBuilder.BuildTicket();
        }
    }
}