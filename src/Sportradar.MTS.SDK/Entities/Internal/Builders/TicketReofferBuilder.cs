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
    /// Implementation of the <see cref="ITicketReofferBuilder"/>
    /// </summary>
    /// <seealso cref="ITicketReofferBuilder" />
    internal class TicketReofferBuilder : ITicketReofferBuilder
    {
        private readonly ISimpleBuilderFactory _builderFactory;
        private ITicket _ticket;
        private ITicketResponse _ticketResponse;
        private string _newTicketId;
        private long _newStake;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketReofferBuilder"/> class
        /// </summary>
        /// <param name="builderFactory">The <see cref="ISimpleBuilderFactory"/> used to construct entity builders</param>
        internal TicketReofferBuilder(ISimpleBuilderFactory builderFactory)
        {
            Guard.Argument(builderFactory, nameof(builderFactory)).NotNull();

            _builderFactory = builderFactory;
        }

        /// <summary>
        /// Sets the original ticket and the ticket response
        /// </summary>
        /// <param name="ticket">The original ticket</param>
        /// <param name="ticketResponse">The ticket response from which the stake info will be used</param>
        /// <param name="newTicketId">The new reoffer ticket id</param>
        /// <returns>Returns the <see cref="ITicketReofferBuilder" /></returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        public ITicketReofferBuilder Set(ITicket ticket, ITicketResponse ticketResponse, string newTicketId = null)
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
        /// <param name="newTicketId">The new reoffer ticket id</param>
        /// <returns>Returns the <see cref="ITicketReofferBuilder"/></returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        public ITicketReofferBuilder Set(ITicket ticket, long newStake, string newTicketId = null)
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
                ? BuildReofferTicket(_builderFactory, _ticket, _ticketResponse, _newTicketId)
                : BuildReofferTicket(_builderFactory, _ticket, _newStake, _newTicketId);
        }

        /// <summary>
        /// Builds the reoffer ticket based on the original ticket and the ticket response
        /// </summary>
        /// <param name="builderFactory">A <see cref="SimpleBuilderFactory"/> used to construct entity builders</param>
        /// <param name="orgTicket">The original ticket</param>
        /// <param name="orgTicketResponse">The ticket response from which the stake info will be used</param>
        /// <param name="newTicketId">The new reoffer ticket id</param>
        /// <exception cref="ArgumentNullException">Ticket and TicketResponse are mandatory</exception>
        /// <returns>Returns the <see cref="ITicket"/> representing the reoffer</returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        /// <exception cref="ArgumentException">Only tickets with exactly 1 bet are supported</exception>
        private static ITicket BuildReofferTicket(ISimpleBuilderFactory builderFactory, ITicket orgTicket, ITicketResponse orgTicketResponse, string newTicketId = null)
        {
            if (orgTicket == null)
            {
                throw new ArgumentNullException(nameof(orgTicket));
            }
            if (orgTicketResponse == null)
            {
                throw new ArgumentNullException(nameof(orgTicketResponse));
            }
            if (orgTicket.Bets.Count() != 1)
            {
                throw new ArgumentException("Only tickets with exactly 1 bet are supported.");
            }
            if (orgTicketResponse.BetDetails.Any(a => a.Reoffer == null))
            {
                throw new ArgumentException("Response bet details are missing Reoffer info.");
            }

            if (orgTicket.Bets.Count() == 1)
            {
                return BuildReofferTicket(builderFactory, orgTicket, orgTicketResponse.BetDetails.First().Reoffer.Stake, newTicketId);
            }

            var reofferTicketBuilder = InitBuilder(builderFactory, orgTicket, newTicketId);

            foreach (var ticketBet in orgTicket.Bets)
            {
                var responseBetDetail = orgTicketResponse.BetDetails.First(f => f.BetId == ticketBet.Id);

                if (responseBetDetail == null)
                {
                    throw new ArgumentException($"Ticket response is missing a bet details for the bet {ticketBet.Id}");
                }

                var newBetBuilder = builderFactory.CreateBetBuilder()
                    .SetBetId(ticketBet.Id + "R")
                    .SetReofferRefId(ticketBet.Id);

                reofferTicketBuilder.AddBet(BuilderHelper.BuildBetFromExisting(newBetBuilder, ticketBet, responseBetDetail.Reoffer.Stake));
            }
            return reofferTicketBuilder.BuildTicket();
        }

        /// <summary>
        /// Builds the reoffer ticket based on the original ticket and the ticket response
        /// </summary>
        /// <param name="builderFactory">A <see cref="SimpleBuilderFactory"/> used to construct entity builders</param>
        /// <param name="orgTicket">The original ticket</param>
        /// <param name="newStake">The new stake value which will be used to set bet stake</param>
        /// <param name="newTicketId">The new reoffer ticket id</param>
        /// <returns>Returns the <see cref="ITicket"/> representing the reoffer</returns>
        /// <remarks>Only tickets with exactly 1 bet are supported</remarks>
        /// <exception cref="ArgumentNullException">Ticket and new stake are mandatory</exception>
        /// <exception cref="ArgumentException">Only tickets with exactly 1 bet are supported</exception>
        private static ITicket BuildReofferTicket(ISimpleBuilderFactory builderFactory, ITicket orgTicket, long newStake, string newTicketId = null)
        {
            if (orgTicket == null)
            {
                throw new ArgumentNullException(nameof(orgTicket));
            }
            if (orgTicket.Bets.Count() != 1)
            {
                throw new ArgumentException("Only tickets with exactly 1 bet are supported.");
            }
            if (newStake <= 0)
            {
                throw new ArgumentException("New stake info is invalid.");
            }

            var reofferTicketBuilder = InitBuilder(builderFactory, orgTicket, newTicketId);

            foreach (var ticketBet in orgTicket.Bets)
            {
                var newBetBuilder = builderFactory.CreateBetBuilder()
                    .SetBetId(ticketBet.Id + "R")
                    .SetReofferRefId(ticketBet.Id);

                reofferTicketBuilder.AddBet(BuilderHelper.BuildBetFromExisting(newBetBuilder, ticketBet, newStake));
            }
            return reofferTicketBuilder.BuildTicket();
        }

        private static ITicketBuilder InitBuilder(ISimpleBuilderFactory builderFactory, ITicket orgTicket, string newTicketId = null)
        {
            var reofferTicketBuilder = builderFactory.CreateTicketBuilder()
                .SetTicketId(string.IsNullOrEmpty(newTicketId) ? orgTicket.TicketId + "R" : newTicketId)
                .SetSender(orgTicket.Sender)
                .SetTestSource(orgTicket.TestSource)
                .SetReofferId(orgTicket.TicketId);

            if (orgTicket.LastMatchEndTime.HasValue)
            {
                reofferTicketBuilder.SetLastMatchEndTime(orgTicket.LastMatchEndTime.Value);
            }

            if (orgTicket.OddsChange.HasValue)
            {
                reofferTicketBuilder.SetOddsChange(orgTicket.OddsChange.Value);
            }

            return reofferTicketBuilder;
        }
    }
}