/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces.CustomBet;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract for classes building a <see cref="ICustomBetSelectionBuilder" />
    /// Implements the <see cref="ISdkTicketBuilder" />
    /// </summary>
    /// <seealso cref="ISdkTicketBuilder" />
    public interface ICustomBetSelectionBuilder
    {
        /// <summary>
        /// Sets event id to the provided <see cref="string"/>
        /// </summary>
        /// <param name="eventId">A <see cref="string"/> representing the event id.</param>
        /// <returns>The <see cref="ICustomBetSelectionBuilder"/> instance used to set additional values.</returns>
        ICustomBetSelectionBuilder SetEventId(string eventId);

        /// <summary>
        /// Sets market id to the provided value
        /// </summary>
        /// <param name="marketId">A value representing the market id.</param>
        /// <returns>The <see cref="ICustomBetSelectionBuilder"/> instance used to set additional values.</returns>
        ICustomBetSelectionBuilder SetMarketId(int marketId);

        /// <summary>
        /// Sets specifiers to the provided value
        /// </summary>
        /// <param name="specifiers">A value representing the specifiers.</param>
        /// <returns>The <see cref="ICustomBetSelectionBuilder"/> instance used to set additional values.</returns>
        ICustomBetSelectionBuilder SetSpecifiers(string specifiers);

        /// <summary>
        /// Sets outcome id to the provided value
        /// </summary>
        /// <param name="outcomeId">A value representing the outcome id.</param>
        /// <returns>The <see cref="ICustomBetSelectionBuilder"/> instance used to set additional values.</returns>
        ICustomBetSelectionBuilder SetOutcomeId(string outcomeId);

        /// <summary>
        /// Builds and returns a <see cref="ISelection"/> instance
        /// </summary>
        /// <returns>The constructed <see cref="ISelection"/> instance.</returns>
        ISelection Build();

        /// <summary>
        /// Builds and returns a <see cref="ISelection"/> instance
        /// </summary>
        /// <returns>The constructed <see cref="ISelection"/> instance.</returns>
        ISelection Build(string eventId, int marketId, string specifiers, string outcomeId);
    }
}
