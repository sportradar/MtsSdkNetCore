/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="ITicketCashoutBuilder"/>
    /// </summary>
    /// <seealso cref="ITicketCashoutBuilder" />
    internal class TicketCashoutBuilder : ITicketCashoutBuilder
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
        /// The cashout stake
        /// </summary>
        private long? _stake;

        /// <summary>
        /// The cashout percent
        /// </summary>
        private int? _percent;

        /// <summary>
        /// The list of bet cashouts
        /// </summary>
        private List<IBetCashout> _betCashouts;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCashoutBuilder"/> class
        /// </summary>
        /// <param name="config">The <see cref="ISdkConfiguration"/> providing configuration for the associated sdk instance</param>
        internal TicketCashoutBuilder(ISdkConfiguration config)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            _bookmakerId = config.BookmakerId;
            _stake = null;
            _percent = null;
            _betCashouts = null;
        }

        #region Obsolete_members
        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCashoutBuilder"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        [Obsolete]
        internal TicketCashoutBuilder(int bookmakerId = 0)
        {
            _bookmakerId = bookmakerId;
        }

        /// <summary>
        /// The <see cref="SdkConfigurationSection"/> loaded from app.config
        /// </summary>
        [Obsolete]
        private static ISdkConfigurationSection _section;

        /// <summary>
        /// Value indicating whether an attempt to load the <see cref="SdkConfigurationSection"/> was already made
        /// </summary>
        [Obsolete]
        private static bool _sectionLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCashoutBuilder"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <returns>Returns an <see cref="ITicketCashoutBuilder"/></returns>
        [Obsolete("Method Create(...) is obsolete. Please use the appropriate method on IBuilderFactory interface which can be obtained through MtsSdk instance")]
        public static ITicketCashoutBuilder Create(int bookmakerId = 0)
        {
            if (!_sectionLoaded)
            {
                SdkConfigurationSection.TryGetSection(out _section);
                _sectionLoaded = true;
            }

            if (_section != null && bookmakerId == 0)
            {
                try
                {
                    var config = SdkConfigurationSection.GetSection();
                    bookmakerId = config.BookmakerId;
                }
                catch (Exception)
                {
                    // if exists, try to load, otherwise user must explicitly set it
                }
            }
            return new TicketCashoutBuilder(bookmakerId);
        }
        #endregion

        /// <summary>
        /// Sets the ticket id to Cashout
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder" /></returns>
        /// <value>Unique ticket id (in the client's system)</value>
        public ITicketCashoutBuilder SetTicketId(string ticketId)
        {
            if (!TicketHelper.ValidateTicketId(ticketId))
            {
                throw new ArgumentException("TicketId not valid");
            }
            _ticketId = ticketId;
            return this;
        }

        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder" /></returns>
        public ITicketCashoutBuilder SetBookmakerId(int bookmakerId)
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
        /// <returns>Returns a <see cref="ITicketCashoutBuilder" /></returns>
        public ITicketCashoutBuilder SetCashoutStake(long stake)
        {
            if (stake < 1)
            {
                throw new ArgumentException("Stake not valid.");
            }
            _stake = stake;
            return this;
        }

        /// <summary>
        /// Sets the cashout percent
        /// </summary>
        /// <param name="percent">The cashout percent</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder"/></returns>
        public ITicketCashoutBuilder SetCashoutPercent(int percent)
        {
            if (!TicketHelper.ValidatePercent(percent))
            {
                throw new ArgumentException("Percent not valid.");
            }
            _percent = percent;
            return this;
        }

        /// <summary>
        /// Add the bet cashout
        /// </summary>
        /// <param name="betId">The bet id</param>
        /// <param name="stake">The cashout stake value of the assigned bet (quantity multiplied by 10_000 and rounded to a long value)</param>
        /// <param name="percent">The cashout percent value of the assigned bet (quantity multiplied by 10_000 and rounded to a int value)</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder"/></returns>
        public ITicketCashoutBuilder AddBetCashout(string betId, long stake, int? percent)
        {
            if (!TicketHelper.ValidateTicketId(betId))
            {
                throw new ArgumentException("BetId not valid");
            }
            if (stake < 1)
            {
                throw new ArgumentException("Stake not valid");
            }
            if (!TicketHelper.ValidatePercent(percent))
            {
                throw new ArgumentException("Percent not valid");
            }
            if (_betCashouts == null)
            {
                _betCashouts = new List<IBetCashout>();
            }
            if (_betCashouts.Any(a => a.BetId.Equals(betId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("BetId already in list.");
            }
            _betCashouts.Add(new BetCashout(betId, stake, percent));
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ITicketCashout" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketCashout" /></returns>
        public ITicketCashout BuildTicket()
        {
            return new TicketCashout(_ticketId, _bookmakerId, _stake, _percent, _betCashouts);
        }

        /// <summary>
        /// Build a <see cref="ITicketCashout" />
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <param name="stake">The cashout stake</param>
        /// <param name="percent">The cashout percent</param>
        /// <returns>Return an <see cref="ITicketCashout"/></returns>
        public ITicketCashout BuildTicket(string ticketId, int bookmakerId, long stake, int? percent)
        {
            if (!TicketHelper.ValidateTicketId(_ticketId))
            {
                throw new ArgumentException("TicketId not valid.");
            }
            if (bookmakerId < 1)
            {
                throw new ArgumentException("BookmakerId not valid.");
            }
            if (stake < 1)
            {
                throw new ArgumentException("Stake not valid.");
            }
            if (!TicketHelper.ValidatePercent(percent))
            {
                throw new ArgumentException("Percent not valid.");
            }
            return new TicketCashout(ticketId, bookmakerId, stake, percent, null);
        }
    }
}