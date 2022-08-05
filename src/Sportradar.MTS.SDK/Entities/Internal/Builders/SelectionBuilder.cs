/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Dawn;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Exceptions;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Cache;
using Sportradar.MTS.SDK.Entities.Internal.Enums;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// Class SelectionBuilder
    /// </summary>
    /// <seealso cref="ISelectionBuilder" />
    internal class  SelectionBuilder : ISelectionBuilder
    {
        private static readonly ILogger ExecutionLog = SdkLoggerFactory.GetLoggerForExecution(typeof(SelectionBuilder));

        private readonly IMarketDescriptionProvider _marketDescriptionProvider;
        private readonly ISdkConfiguration _config;
        /// <summary>
        /// The event identifier
        /// </summary>
        private string _eventId;

        /// <summary>
        /// The selection identifier
        /// </summary>
        private string _selectionId;

        /// <summary>
        /// The odds
        /// </summary>
        private int? _odds;

        /// <summary>
        /// The is banker
        /// </summary>
        private bool _isBanker;

        /// <summary>
        /// The is custom bet
        /// </summary>
        private readonly bool _isCustomBet;


        private int? _boostedOdds;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionBuilder"/> class
        /// </summary>
        public SelectionBuilder(IMarketDescriptionProvider marketDescriptionProvider, ISdkConfiguration config, bool isCustomBet)
        {
            Guard.Argument(marketDescriptionProvider, nameof(marketDescriptionProvider)).NotNull();
            Guard.Argument(config, nameof(config)).NotNull();

            _marketDescriptionProvider = marketDescriptionProvider;
            _isBanker = false;
            _config = config;
            _isCustomBet = isCustomBet;
        }

        /// <summary>
        /// Sets the Betradar event (match or outright) id
        /// </summary>
        /// <param name="eventId">The event identifier</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        public ISelectionBuilder SetEventId(long eventId)
        {
            _eventId = eventId.ToString();
            ValidateData(false, true);
            return this;
        }

        /// <summary>
        /// Sets the Betradar event (match or outright) id
        /// </summary>
        /// <param name="eventId">The event identifier</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        public ISelectionBuilder SetEventId(string eventId)
        {
            _eventId = eventId;
            ValidateData(false, true);
            return this;
        }

        /// <summary>
        /// Sets the selection id
        /// </summary>
        /// <param name="id">The identifier</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <value>Should be composed according to specification</value>
        public ISelectionBuilder SetId(string id)
        {
            _selectionId = id;
            ValidateData(true);
            return this;
        }

        /// <summary>
        /// Sets the identifier lo
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="subType">Type of the sub</param>
        /// <param name="sov">The sov</param>
        /// <param name="selectionId">The selection id</param>
        /// <returns>ISelectionBuilder</returns>
        public ISelectionBuilder SetIdLo(int type, int subType, string sov, string selectionId)
        {
            if (subType < 0)
            {
                subType = 0;
            }
            if (string.IsNullOrEmpty(sov))
            {
                sov = "*";
            }
            _selectionId = $"live:{type}/{subType}/{sov}";
            if (!string.IsNullOrEmpty(selectionId))
            {
                _selectionId += "/" + selectionId;
            }
            ValidateData(true);
            return this;
        }

        /// <summary>
        /// Sets the identifier lcoo
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="sportId">The sport identifier</param>
        /// <param name="sov">The sov</param>
        /// <param name="selectionId">The selection ids</param>
        /// <returns>ISelectionBuilder</returns>
        public ISelectionBuilder SetIdLcoo(int type, int sportId, string sov, string selectionId)
        {
            if (string.IsNullOrEmpty(sov))
            {
                sov = "*";
            }
            _selectionId = $"lcoo:{type}/{sportId}/{sov}";
            if (!string.IsNullOrEmpty(selectionId))
            {
                _selectionId += "/" + selectionId;
            }
            ValidateData(true);
            return this;
        }

        /// <summary>
        /// Sets the selection id for UOF
        /// </summary>
        /// <param name="product">The product to be used</param>
        /// <param name="sportId">The UF sport id</param>
        /// <param name="marketId">The UF market id</param>
        /// <param name="selectionId">The selection id</param>
        /// <param name="specifiers">The UF array of specifiers represented as string separated with '|'</param>
        /// <param name="sportEventStatus">The UF sport event status properties</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <value>Should be composed according to specification</value>
        /// <example>
        /// SetIdUof(1, "sr:sport:1", 101, "10", "total=3.0|playerid=sr:player:10201");
        /// </example>
        public ISelectionBuilder SetIdUof(int product, string sportId, int marketId, string selectionId, string specifiers, IReadOnlyDictionary<string, object> sportEventStatus)
        {
            IDictionary<string, string> specs = null;
            if (!string.IsNullOrEmpty(specifiers))
            {
                specs = new Dictionary<string, string>();
                foreach (var spec in specifiers.Split('|'))
                {
                    var s = spec.Split('=');
                    specs.Add(s[0], s[1]);
                }
            }
            return SetIdUof(product, sportId, marketId, selectionId, (IReadOnlyDictionary<string, string>) specs, sportEventStatus);
        }

        /// <summary>
        /// Sets the selection id for UOF
        /// </summary>
        /// <param name="product">The product to be used</param>
        /// <param name="sportId">The UF sport id</param>
        /// <param name="marketId">The UF market id</param>
        /// <param name="selectionId">The selection id</param>
        /// <param name="specifiers">The array of specifiers</param>
        /// <param name="sportEventStatus">The UF sport event status properties</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <value>Should be composed according to specification</value>
        public ISelectionBuilder SetIdUof(int product, string sportId, int marketId, string selectionId, IReadOnlyDictionary<string, string> specifiers, IReadOnlyDictionary<string, object> sportEventStatus)
        {
            if (!URN.TryParse(sportId, out _))
            {
                throw new ArgumentException("SportId is not valid.");
            }
            if (product < 1)
            {
                throw new ArgumentException("Product is not valid.");
            }

            _selectionId = $"uof:{product}/{sportId}/{marketId}";
            if (!string.IsNullOrEmpty(selectionId))
            {
                _selectionId += "/" + selectionId;
            }
            var newSpecifiers = HandleMarketDescription(product, sportId, marketId, specifiers, sportEventStatus);
            if (newSpecifiers != null && newSpecifiers.Any())
            {
                var specs = newSpecifiers.Aggregate(string.Empty, (s, pair) => s + "&" + pair.Key + "=" + pair.Value);
                _selectionId += "?" + specs.Substring(1);
            }
            ValidateData(true);
            return this;
        }

        /// <summary>
        /// Sets the odds multiplied by 10000 and rounded to int value
        /// </summary>
        /// <param name="odds">The odds value to be set</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        public ISelectionBuilder SetOdds(int odds)
        {
            _odds = odds;
            ValidateData(false, false, !_isCustomBet);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ISelection" /> properties
        /// </summary>
        /// <param name="eventId">The event id</param>
        /// <param name="id">The selection id</param>
        /// <param name="odds">The odds value to be set</param>
        /// <param name="isBanker"></param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        public ISelectionBuilder Set(long eventId, string id, int? odds, bool isBanker)
        {
            return Set(eventId.ToString(), id, odds, isBanker);
        }

        /// <summary>
        /// Sets the <see cref="ISelection" /> properties
        /// </summary>
        /// <param name="eventId">The event id</param>
        /// <param name="id">The selection id</param>
        /// <param name="odds">The odds value to be set</param>
        /// <param name="isBanker"></param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        public ISelectionBuilder Set(string eventId, string id, int? odds, bool isBanker)
        {
            _eventId = eventId;
            _selectionId = id;
            _odds = odds;
            _isBanker = isBanker;
            ValidateData(true, true, !_isCustomBet);
            return this;
        }

        /// <summary>
        /// Sets the banker property
        /// </summary>
        /// <param name="isBanker">if set to <c>true</c> [is banker]</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        public ISelectionBuilder SetBanker(bool isBanker)
        {
            _isBanker = isBanker;
            return this;
        }

        /// <summary>
        /// Sets the boosted odds multiplied by 10000 and rounded to int value
        /// </summary>
        /// <param name="boostedOdds">The boosted odds value to be set</param>
        /// <returns>Returns a <see cref="ISelectionBuilder" /></returns>
        public ISelectionBuilder SetBoostedOdds(int? boostedOdds)
        {
            _boostedOdds = boostedOdds;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ISelection" />
        /// </summary>
        /// <returns>Returns a <see cref="ITicketBuilder" /></returns>
        public ISelection Build()
        {
            ValidateData(true, true, !_isCustomBet);
            return new Selection(_eventId, _selectionId, _odds, _isBanker, _boostedOdds);
        }

        private void ValidateData(bool id = false, bool eventId = false, bool odds = false)
        {
            if (id && (string.IsNullOrEmpty(_selectionId) || !TicketHelper.ValidateStringId(_selectionId, false, true, 1, 1000)))
            {
                throw new ArgumentException($"Id {_selectionId} not valid.");
            }
            if (eventId && (string.IsNullOrEmpty(_eventId) || !TicketHelper.ValidateStringId(_eventId, false, true, 1, 100)))
            {
                throw new ArgumentException($"EventId {_eventId} not valid.");
            }
            if (odds && (_odds == null || !(_odds >= 10000 && _odds <= 1000000000)))
            {
                throw new ArgumentException($"Odds {_odds} not valid.");
            }
        }

        private IReadOnlyDictionary<string, string> HandleMarketDescription(int productId,
                                                                            string sportId,
                                                                            int marketId,
                                                                            IReadOnlyDictionary<string, string> specifiers,
                                                                            IReadOnlyDictionary<string, object> sportEventStatus)
        {
            Guard.Argument(marketId, nameof(marketId)).Positive();

            if (!_config.ProvideAdditionalMarketSpecifiers)
            {
                return specifiers;
            }

            MarketDescriptionCacheItem marketDescription = null;
            try
            {
                marketDescription = _marketDescriptionProvider.GetMarketDescriptorAsync(marketId, null).Result;
            }
            catch (Exception ex)
            {
                HandleProviderException(ex, productId, sportId, marketId);
            }

            if (marketDescription == null)
            {
                ExecutionLog.LogInformation($"No market description found for marketId={marketId}, sportId={sportId}, productId={productId}.");
                return specifiers;
            }

            //handle market 215
            if (marketId == 215)
            {
                return HandleMarket215(specifiers, sportEventStatus);
            }

            //handle $score sov_template
            return HandleScoreSovTemplate(marketDescription, productId, sportId, specifiers, sportEventStatus);
        }

        private void HandleProviderException(Exception ex, int productId, string sportId, int marketId)
        {
            var baseEx = ex.GetBaseException();
            if (baseEx.InnerException != null)
            {
                baseEx = baseEx.InnerException;
                if (baseEx.InnerException != null)
                {
                    baseEx = baseEx.InnerException;
                }
            }
            if (baseEx is CacheItemNotFoundException)
            {
                ExecutionLog.LogWarning($"No market description found for marketId={marketId}, sportId={sportId}, productId={productId}. Ex: {baseEx.Message}");
            }
            else if (baseEx is CommunicationException)
            {
                ExecutionLog.LogWarning($"Exception during fetching market descriptions. Ex: {baseEx.Message}");
            }
            else
            {
                ExecutionLog.LogWarning("Exception during fetching market descriptions.", baseEx);
            }
        }

        private IReadOnlyDictionary<string, string> HandleMarket215(IReadOnlyDictionary<string, string> specifiers, IReadOnlyDictionary<string, object> sportEventStatus)
        {
            var newSpecifiers = specifiers == null
                ? new Dictionary<string, string>()
                : specifiers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (sportEventStatus == null)
            {
                throw new ArgumentException("SportEventStatus is missing.");
            }

            if (!sportEventStatus.ContainsKey("CurrentServer"))
            {
                throw new ArgumentException("SportEventStatus is missing CurrentServer.");
            }

            newSpecifiers.Add("$server", sportEventStatus["CurrentServer"].ToString());

            return new ReadOnlyDictionary<string, string>(newSpecifiers);
        }

        private IReadOnlyDictionary<string, string> HandleScoreSovTemplate(MarketDescriptionCacheItem marketDescription, int productId, string sportId, IReadOnlyDictionary<string, string> specifiers,
            IReadOnlyDictionary<string, object> sportEventStatus)
        {
            if (marketDescription.Mappings == null)
            {
                ExecutionLog.LogInformation($"Market description {marketDescription.Id} has no mapping.");
                return specifiers;
            }
            var marketMapping = marketDescription.Mappings.FirstOrDefault(f => f.ProductId == productId && Equals(f.SportId, URN.Parse(sportId)));
            if (marketMapping == null || marketMapping.ProductId == 0)
            {
                ExecutionLog.LogInformation($"Market description {marketDescription.Id} has no mapping sportId={sportId}, productId={productId}.");
                return specifiers;
            }

            if (string.IsNullOrEmpty(marketMapping.SovTemplate) || !marketMapping.SovTemplate.Contains("{$score}"))
            {
                return specifiers;
            }

            if (sportEventStatus == null)
            {
                throw new ArgumentException("SportEventStatus is missing.");
            }

            if (!sportEventStatus.ContainsKey("HomeScore"))
            {
                throw new ArgumentException("SportEventStatus is missing HomeScore property.");
            }

            if (!sportEventStatus.ContainsKey("AwayScore"))
            {
                throw new ArgumentException("SportEventStatus is missing AwayScore property.");
            }

            var newSpecifiers = specifiers == null
                ? new Dictionary<string, string>()
                : specifiers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Debug.Assert(newSpecifiers != null, "newSpecifiers != null");
            newSpecifiers.Add("$score", $"{sportEventStatus["HomeScore"]}:{sportEventStatus["AwayScore"]}");

            return new ReadOnlyDictionary<string, string>(newSpecifiers);
        }
    }
}