/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Builders;
using System;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Dawn;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="ITicketNonSrSettleBuilder"/>
    /// </summary>
    /// <seealso cref="ITicketNonSrSettleBuilder" />
    public class TicketNonSrSettleBuilder : ITicketNonSrSettleBuilder
    {
        /// <summary>
        /// The ticket identifier
        /// </summary>
        private string _ticketId;

        /// <summary>
        /// The bookmaker identifier
        /// </summary>
        private int _bookmakerId;

        /// <summary>
        /// The non-sportradar settlement stake
        /// </summary>
        private long _stake;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketNonSrSettleBuilder"/> class
        /// </summary>
        /// <param name="config">The <see cref="ISdkConfiguration"/> providing configuration for the associated sdk instance</param>
        internal TicketNonSrSettleBuilder(ISdkConfiguration config)
        {
            Guard.Argument(config).NotNull();

            _bookmakerId = config.BookmakerId;
        }

        public ITicketNonSrSettle BuildTicket()
        {
            return new TicketNonSrSettle(_ticketId, _bookmakerId, _stake);
        }

        public ITicketNonSrSettle BuildTicket(string ticketId, int bookmakerId, long stake)
        {

            if (!TicketHelper.ValidateTicketId(_ticketId))
            {
                throw new ArgumentException("TicketId not valid.");
            }
            if (bookmakerId < 1)
            {
                throw new ArgumentException("BookmakerId not valid.");
            }
            if (stake < 0)
            {
                throw new ArgumentException("Stake not valid.");
            }

            return new TicketNonSrSettle(ticketId, bookmakerId, stake);
        }

        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketNonSrSettleBuilder" /></returns>
        public ITicketNonSrSettleBuilder SetBookmakerId(int bookmakerId)
        {
            if (bookmakerId < 1)
            {
                throw new ArgumentException("BookmakerId not valid.");
            }
            _bookmakerId = bookmakerId;
            return this;
        }

        /// <summary>
        /// Sets the cashout stake
        /// </summary>
        /// <param name="stake">The cashout stake</param>
        /// <returns>Returns a <see cref="ITicketNonSrSettleBuilder" /></returns>
        public ITicketNonSrSettleBuilder SetNonSrSettleStake(long stake)
        {
            if (stake < 0)
            {
                throw new ArgumentException("Stake not valid.");
            }
            _stake = stake;
            return this;
        }

        /// <summary>
        /// Sets the ticket id to Cashout
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketNonSrSettleBuilder" /></returns>
        /// <value>Unique ticket id (in the client's system)</value>
        public ITicketNonSrSettleBuilder SetTicketId(string ticketId)
        {
            if (!TicketHelper.ValidateTicketId(ticketId))
            {
                throw new ArgumentException("TicketId not valid");
            }
            _ticketId = ticketId;
            return this;
        }
    }
}
