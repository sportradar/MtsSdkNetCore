/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using System;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes implementing builder for <see cref="ITicket" />
    /// </summary>
    public interface ITicketBuilder : ISdkTicketBuilder
    {
        /// <summary>
        /// Sets the ticket id
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetTicketId(string ticketId);

        /// <summary>
        /// Gets the bets
        /// </summary>
        /// <returns>Returns all the bets</returns>
        IEnumerable<IBet> GetBets();

        /// <summary>
        /// Adds the <see cref="IBet"/>
        /// </summary>
        /// <param name="bet">A <see cref="IBet"/> to be added to this ticket</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder AddBet(IBet bet);

        /// <summary>
        /// Sets the reoffer id
        /// </summary>
        /// <param name="reofferId">The reoffer id</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetReofferId(string reofferId);

        /// <summary>
        /// Sets the alternative stake reference ticket id
        /// </summary>
        /// <param name="altStakeRefId">The alt stake reference id</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetAltStakeRefId(string altStakeRefId);

        /// <summary>
        /// Sets the test source
        /// </summary>
        /// <param name="isTest">if set to <c>true</c> [is test]</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetTestSource(bool isTest);

        /// <summary>
        /// Sets the odds change
        /// </summary>
        /// <param name="type">The <see cref="OddsChangeType"/> to be set</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetOddsChange(OddsChangeType type);

        /// <summary>
        /// Sets the sender
        /// </summary>
        /// <param name="sender">The ticket sender</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetSender(ISender sender);

        /// <summary>
        /// Sets the expected total number of generated combinations on this ticket (optional, default null). If present, it is used to validate against actual number of generated combinations.
        /// </summary>
        /// <param name="totalCombinations">The expected total number of generated combinations</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetTotalCombinations(int totalCombinations);

        /// <summary>
        /// Sets end time of last (non Sportradar) match on ticket
        /// </summary>
        /// <param name="lastMatchEndTime">End time of last (non Sportradar) match on ticket</param>
        /// <returns>Returns a <see cref="ITicketBuilder"/></returns>
        ITicketBuilder SetLastMatchEndTime(DateTime lastMatchEndTime);

        /// <summary>
        /// Builds the <see cref="ITicket" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicket"/></returns>
        ITicket BuildTicket();
    }
}
