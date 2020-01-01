/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="ITicketCancelBuilder"/>
    /// </summary>
    /// <seealso cref="ITicketCancelBuilder" />
    public class TicketCancelBuilder : ITicketCancelBuilder
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
        /// The code
        /// </summary>
        private TicketCancellationReason _code;

        /// <summary>
        /// The percent
        /// </summary>
        private int? _percent;

        /// <summary>
        /// The bet cancels
        /// </summary>
        private List<IBetCancel> _betCancels;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCancelBuilder"/> class
        /// </summary>
        /// <param name="config">A <see cref="ISdkConfiguration"/> providing configuration for the associated sdk instance</param>
        internal TicketCancelBuilder(ISdkConfiguration config)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            _bookmakerId = config.BookmakerId;
            _percent = null;
            _betCancels = null;
        }

        #region Obsolete_members
        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCancelBuilder"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        internal TicketCancelBuilder(int bookmakerId = 0)
        {
            _bookmakerId = bookmakerId;
        }

        /// <summary>
        /// The <see cref="SdkConfigurationSection"/> loaded from app.config
        /// </summary>
        private static ISdkConfigurationSection _section;

        /// <summary>
        /// Value indicating whether an attempt to load the <see cref="_section"/> was already made
        /// </summary>
        private static bool _sectionLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketCancelBuilder"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <returns>Returns an <see cref="ITicketCancelBuilder"/></returns>
        [Obsolete("Method Create(...) is obsolete. Please use the appropriate method on IBuilderFactory interface which can be obtained through MtsSdk instance")]
        public static ITicketCancelBuilder Create(int bookmakerId = 0)
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
            return new TicketCancelBuilder(bookmakerId);
        }
        #endregion

        /// <summary>
        /// Sets the ticket id to cancel
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketCancelBuilder" /></returns>
        /// <value>Unique ticket id (in the client's system)</value>
        public ITicketCancelBuilder SetTicketId(string ticketId)
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
        /// <returns>Returns a <see cref="ITicketCancelBuilder" /></returns>
        public ITicketCancelBuilder SetBookmakerId(int bookmakerId)
        {
            if (bookmakerId < 1)
            {
                throw new ArgumentException("BookmakerId not valid");
            }
            _bookmakerId = bookmakerId;
            return this;
        }

        /// <summary>
        /// Sets the cancellation code
        /// </summary>
        /// <param name="code">The <see cref="TicketCancellationReason" /></param>
        /// <returns>Returns a <see cref="ITicketCancelBuilder" /></returns>
        public ITicketCancelBuilder SetCode(TicketCancellationReason code)
        {
            _code = code;
            return this;
        }

        /// <summary>
        /// Sets the percent of cancellation
        /// </summary>
        /// <param name="percent">The percent of cancellation</param>
        /// <returns>Returns a <see cref="ITicketCancelBuilder"/></returns>
        public ITicketCancelBuilder SetCancelPercent(int percent)
        {
            if (!TicketHelper.ValidatePercent(percent))
            {
                throw new ArgumentException("Percent not valid");
            }
            _percent = percent;
            return this;
        }

        /// <summary>
        /// Add the bet cancel
        /// </summary>
        /// <param name="betId">The bet id</param>
        /// <param name="percent">The cancel percent value of the assigned bet (quantity multiplied by 10_000 and rounded to a int value)</param>
        /// <returns>Returns a <see cref="ITicketCancelBuilder"/></returns>
        public ITicketCancelBuilder AddBetCancel(string betId, int? percent)
        {
            if (!TicketHelper.ValidateTicketId(betId))
            {
                throw new ArgumentException("BetId not valid");
            }
            if (!TicketHelper.ValidatePercent(percent))
            {
                throw new ArgumentException("Percent not valid");
            }
            if (_betCancels == null)
            {
                _betCancels = new List<IBetCancel>();
            }
            if (_betCancels.Count >= 50)
            {
                throw new ArgumentException("List size limit reached. Only 50 allowed.");
            }
            if (_betCancels.Any(a => a.BetId.Equals(betId, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ArgumentException("BetId already in list.");
            }
            _betCancels.Add(new BetCancel(betId, percent));
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ITicketCancel" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketCancel" /></returns>
        public ITicketCancel BuildTicket()
        {
            return new TicketCancel(_ticketId, _bookmakerId, _code, _percent, _betCancels);
        }

        /// <summary>
        /// Build a <see cref="ITicketCancel" />
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <param name="code">The <see cref="TicketCancellationReason" /></param>
        /// <returns>Return an <see cref="ITicketCancel"/></returns>
        public ITicketCancel BuildTicket(string ticketId, int bookmakerId, TicketCancellationReason code)
        {
            return new TicketCancel(ticketId, bookmakerId, code, _percent, _betCancels);
        }
    }
}