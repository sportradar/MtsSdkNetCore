/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes building a <see cref="ITicketCashout" />
    /// </summary>
    public interface ITicketCashoutBuilder : ISdkTicketBuilder
    {
        /// <summary>
        /// Sets the ticket id to cashout
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder"/></returns>
        /// <value>Unique ticket id (in the client's system)</value>
        ITicketCashoutBuilder SetTicketId(string ticketId);

        /// <summary>
        /// Sets the bookmaker id (client's id provided by Sportradar)
        /// </summary>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder"/></returns>
        ITicketCashoutBuilder SetBookmakerId(int bookmakerId);

        /// <summary>
        /// Sets the cashout stake
        /// </summary>
        /// <param name="stake">The cashout stake</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder"/></returns>
        ITicketCashoutBuilder SetCashoutStake(long stake);

        /// <summary>
        /// Sets the cashout percent
        /// </summary>
        /// <param name="percent">The cashout percent</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder"/></returns>
        ITicketCashoutBuilder SetCashoutPercent(int percent);

        /// <summary>
        /// Add the bet cashout
        /// </summary>
        /// <param name="betId">The bet id</param>
        /// <param name="stake">The cashout stake value of the assigned bet (quantity multiplied by 10_000 and rounded to a long value)</param>
        /// <param name="percent">The cashout percent value of the assigned bet (quantity multiplied by 10_000 and rounded to a long value)</param>
        /// <returns>Returns a <see cref="ITicketCashoutBuilder"/></returns>
        ITicketCashoutBuilder AddBetCashout(string betId, long stake, int? percent);

        /// <summary>
        /// Build a <see cref="ITicketCashout" />
        /// </summary>
        /// <param name="ticketId">The ticket id</param>
        /// <param name="bookmakerId">The bookmaker id</param>
        /// <param name="stake">The cashout stake</param>
        /// <param name="percent">The cashout percent</param>
        /// <returns>ITicketCashout</returns>
        ITicketCashout BuildTicket(string ticketId, int bookmakerId, long stake, int? percent);

        /// <summary>
        /// Builds the <see cref="ITicketCashout" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketCashout"/></returns>
        ITicketCashout BuildTicket();
    }
}
