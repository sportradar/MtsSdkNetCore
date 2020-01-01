/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Implementation of the <see cref="ITicketReofferCancelBuilder"/>
    /// </summary>
    /// <seealso cref="ITicketReofferCancelBuilder" />
    public class TicketReofferCancelBuilder : ITicketReofferCancelBuilder
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
        /// Initializes a new instance of the <see cref="TicketReofferCancelBuilder"/> class
        /// </summary>
        /// <param name="config">The <see cref="ISdkConfiguration"/> providing configuration for associated sdk instance</param>
        internal TicketReofferCancelBuilder(ISdkConfiguration config)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            _bookmakerId = config.BookmakerId;
        }

        #region Obsolete_members
        /// <summary>
        /// Initializes a new instance of the <see cref="TicketReofferCancelBuilder"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        internal TicketReofferCancelBuilder(int bookmakerId = 0)
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
        /// Initializes a new instance of the <see cref="TicketReofferCancelBuilder"/> class
        /// </summary>
        /// <param name="bookmakerId">The bookmaker identifier</param>
        /// <returns>Returns an <see cref="ITicketReofferCancelBuilder"/></returns>
        [Obsolete("Method Create(...) is obsolete. Please use the appropriate method on IBuilderFactory interface which can be obtained through MtsSdk instance")]
        public static ITicketReofferCancelBuilder Create(int bookmakerId = 0)
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
            return new TicketReofferCancelBuilder(bookmakerId);
        }
        #endregion

        /// <summary>
        /// Sets the reoffer ticket id to cancel
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketReofferCancelBuilder" /></returns>
        /// <value>Unique ticket id (in the client's system)</value>
        public ITicketReofferCancelBuilder SetTicketId(string ticketId)
        {
            if (!TicketHelper.ValidateTicketId(ticketId))
            {
                throw new ArgumentException("TicketId not valid.");
            }
            _ticketId = ticketId;
            return this;
        }

        /// <summary>
        /// Get the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketCancelBuilder" /></returns>
        public ITicketReofferCancelBuilder SetBookmakerId(int bookmakerId)
        {
            if (bookmakerId < 1)
            {
                throw new ArgumentException("BookmakerId not valid.");
            }
            _bookmakerId = bookmakerId;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ITicketReofferCancel" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketReofferCancel" /></returns>
        public ITicketReofferCancel BuildTicket()
        {
            return new TicketReofferCancel(_ticketId, _bookmakerId);
        }

        /// <summary>
        /// Build a <see cref="ITicketCancel" />
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Return an <see cref="ITicketCancel"/></returns>
        public ITicketReofferCancel BuildTicket(string ticketId, int bookmakerId)
        {
            if (!TicketHelper.ValidateTicketId(_ticketId))
            {
                throw new ArgumentException("TicketId not valid.");
            }
            if (bookmakerId < 1)
            {
                throw new ArgumentException("BookmakerId not valid.");
            }
            return new TicketReofferCancel(ticketId, bookmakerId);
        }
    }
}