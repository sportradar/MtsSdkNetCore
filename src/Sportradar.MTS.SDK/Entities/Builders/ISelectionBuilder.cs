/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    ///     Defines a contract for classes implementing builder for <see cref="ISelection" />
    /// </summary>
    public interface ISelectionBuilder
    {
        /// <summary>
        ///     Sets the Betradar event (match or outright) id
        /// </summary>
        /// <param name="eventId">The event identifier</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        ISelectionBuilder SetEventId(string eventId);

        /// <summary>
        ///     Sets the selection id
        /// </summary>
        /// <param name="id">The identifier</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <value>Should be composed according to MTS specification</value>
        ISelectionBuilder SetId(string id);

        /// <summary>
        ///     Sets the selection id for LiveOdds
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="subType">The subType</param>
        /// <param name="sov">The special odds value</param>
        /// <param name="selectionId">The selection id</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <value>Should be composed according to MTS specification</value>
        ISelectionBuilder SetIdLo(int type, int subType, string sov, string selectionId);

        /// <summary>
        ///     Sets the selection id for LCOO
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="sportId">The sport id</param>
        /// <param name="sov">The special odds value</param>
        /// <param name="selectionId">The selection id</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <value>Should be composed according to MTS specification</value>
        ISelectionBuilder SetIdLcoo(int type, int sportId, string sov, string selectionId);

        /// <summary>
        ///     Sets the selection id for UOF
        /// </summary>
        /// <param name="product">The product to be used</param>
        /// <param name="sportId">The UF sport id</param>
        /// <param name="marketId">The UF market id</param>
        /// <param name="selectionId">The selection id</param>
        /// <param name="specifiers">The array of specifiers represented as string separated with '|'</param>
        /// <param name="sportEventStatus">The UF sport event status properties</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <value>Should be composed according to MTS specification</value>
        /// <example>
        ///     SetIdUof(Product.LiveOdds, "sr:sport:1", 101, "10", "total=3.0|playerid=sr:player:10201");
        /// </example>
        /// <remarks>Method requires accessToken in configuration and access to https://global.api.betradar.com</remarks>
        ISelectionBuilder SetIdUof(int product, string sportId, int marketId, string selectionId, string specifiers, IReadOnlyDictionary<string, object> sportEventStatus);

        /// <summary>
        ///     Sets the selection id for UOF
        /// </summary>
        /// <param name="product">The product to be used</param>
        /// <param name="sportId">The UF sport id</param>
        /// <param name="marketId">The UF market id</param>
        /// <param name="selectionId">The selection id</param>
        /// <param name="specifiers">The array of specifiers</param>
        /// <param name="sportEventStatus">The UF sport event status properties</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <value>Should be composed according to MTS specification</value>
        /// <remarks>Method requires accessToken in configuration and access to https://global.api.betradar.com</remarks>
        ISelectionBuilder SetIdUof(int product, string sportId, int marketId, string selectionId, IReadOnlyDictionary<string, string> specifiers, IReadOnlyDictionary<string, object> sportEventStatus);

        /// <summary>
        ///     Sets the odds multiplied by 10000 and rounded to int value
        /// </summary>
        /// <param name="odds">The odds value to be set</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        ISelectionBuilder SetOdds(int odds);

        /// <summary>
        ///     Sets the banker property
        /// </summary>
        /// <param name="isBanker">if set to <c>true</c> [is banker]</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        ISelectionBuilder SetBanker(bool isBanker);

        /// <summary>
        /// Sets the boosted odds multiplied by 10000 and rounded to int value
        /// </summary>
        /// <param name="boostedOdds">The boosted odds value to be set</param>
        /// <returns>Returns a <see cref="ISelectionBuilder"/></returns>
        ISelectionBuilder SetBoostedOdds(int? boostedOdds);

        /// <summary>
        ///     Sets the <see cref="ISelection" /> properties
        /// </summary>
        /// <param name="eventId">The event id</param>
        /// <param name="id">The selection id</param>
        /// <param name="odds">The odds value to be set</param>
        /// <param name="isBanker">if set to <c>true</c> [is banker]</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        ISelectionBuilder Set(long eventId, string id, int? odds, bool isBanker);

        /// <summary>
        ///     Sets the <see cref="ISelection" /> properties
        /// </summary>
        /// <param name="eventId">The event id</param>
        /// <param name="id">The selection id</param>
        /// <param name="odds">The odds value to be set</param>
        /// <param name="isBanker">if set to <c>true</c> [is banker]</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        ISelectionBuilder Set(string eventId, string id, int? odds, bool isBanker);

        /// <summary>
        ///     Builds the <see cref="ISelection" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        ISelection Build();
    }
}