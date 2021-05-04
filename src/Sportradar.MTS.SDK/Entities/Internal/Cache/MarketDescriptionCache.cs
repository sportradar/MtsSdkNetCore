/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Exceptions;
using Sportradar.MTS.SDK.Common.Internal.Metrics;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;
using App.Metrics.Health;

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    /// <summary>
    /// A <see cref="IMarketDescriptionCache" /> implementation used to store market descriptors for invariant markets
    /// </summary>
    /// <seealso cref="IDisposable" />
    /// <seealso cref="IMarketDescriptionCache" />
    internal sealed class MarketDescriptionCache : IMarketDescriptionCache, IDisposable, IHealthStatusProvider
    {
        private readonly CacheItemPolicy _cacheItemPolicy;
        private readonly TimeSpan _fetchInterval;
        private readonly TimeSpan _minIntervalTimeout = TimeSpan.FromSeconds(30);
        internal DateTime TimeOfLastFetch;

        /// <summary>
        /// A <see cref="ILogger"/> instance for execution logging
        /// </summary>
        private static readonly ILogger CacheLog = SdkLoggerFactory.GetLoggerForCache(typeof(MarketDescriptionCache));

        /// <summary>
        /// A <see cref="ILogger"/> instance for execution logging
        /// </summary>
        private static readonly ILogger ExecutionLog = SdkLoggerFactory.GetLoggerForExecution(typeof(MarketDescriptionCache));

        /// <summary>
        /// A <see cref="ObjectCache"/> used to store market descriptors
        /// </summary>
        internal readonly ObjectCache Cache;

        /// <summary>
        /// A <see cref="IDataProvider{T}"/> used to fetch market descriptors
        /// </summary>
        private readonly IDataProvider<IEnumerable<MarketDescriptionDTO>> _dataProvider;

        /// <summary>
        /// A <see cref="ISet{CultureInfo}"/> used to store languages for which the data was already fetched (at least once)
        /// </summary>
        private readonly ISet<CultureInfo> _fetchedLanguages = new HashSet<CultureInfo>();

        /// <summary>
        /// A <see cref="SemaphoreSlim"/> instance to synchronize access from multiple threads
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Value indicating whether the current instance was already disposed
        /// </summary>
        private bool _isDisposed;

        private readonly bool _tokenProvided;

        private readonly IMetricsRoot _metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDescriptionCache"/> class
        /// </summary>
        /// <param name="cache">A <see cref="ObjectCache"/> used to store market descriptors</param>
        /// <param name="dataProvider">A <see cref="IDataProvider{T}"/> used to fetch market descriptors</param>
        /// <param name="accessToken">The <see cref="ISdkConfigurationSection.AccessToken"/> used to access UF REST API</param>
        /// <param name="fetchInterval">The fetch interval</param>
        /// <param name="cacheItemPolicy">The cache item policy</param>
        /// <param name="metrics">A <see cref="IMetricsRoot"/> used to record sdk metrics</param>
        public MarketDescriptionCache(ObjectCache cache,
                                      IDataProvider<IEnumerable<MarketDescriptionDTO>> dataProvider,
                                      string accessToken,
                                      TimeSpan fetchInterval,
                                      CacheItemPolicy cacheItemPolicy,
                                      IMetricsRoot metrics = null)
        {
            Guard.Argument(cache, nameof(cache)).NotNull();
            Guard.Argument(dataProvider, nameof(dataProvider)).NotNull();
            Guard.Argument(fetchInterval, nameof(fetchInterval)).Require(fetchInterval.TotalSeconds > 0);
            
            _fetchInterval = fetchInterval;
            _cacheItemPolicy = cacheItemPolicy;
            TimeOfLastFetch = DateTime.MinValue;
            Cache = cache;
            _dataProvider = dataProvider;

            _tokenProvided = !string.IsNullOrEmpty(accessToken);
            var isProvided = _tokenProvided ? string.Empty : " not";

            _metrics = metrics ?? SdkMetricsFactory.MetricsRoot;

            ExecutionLog.LogDebug($"AccessToken for API is{isProvided} provided. It is required only when creating selections for UF markets via method ISelectionBuilder.SetIdUof(). There is no need for it when legacy feeds are used.");
        }

        /// <summary>
        /// Gets the <see cref="MarketDescriptionCacheItem"/> specified by it's id from the local cache
        /// </summary>
        /// <param name="id">The id of the <see cref="MarketDescriptionCacheItem"/> to get</param>
        /// <returns>The <see cref="MarketDescriptionCacheItem"/> retrieved from the cache or a null reference if item is not found</returns>
        private MarketDescriptionCacheItem GetItemFromCache(int id)
        {
            var cacheItem = Cache.GetCacheItem(id.ToString());
            return (MarketDescriptionCacheItem) cacheItem?.Value;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{CultureInfo}"/> containing <see cref="CultureInfo"/> instances from provided <code>requiredTranslations</code>
        /// which translations are not found in the provided <see cref="MarketDescriptionCacheItem"/>
        /// </summary>
        /// <param name="item">The <see cref="MarketDescriptionCacheItem"/> instance, or a null reference</param>
        /// <param name="requiredTranslations">The <see cref="IEnumerable{CultureInfo}"/> specifying the required languages</param>
        /// <returns>A <see cref="IEnumerable{CultureInfo}"/> containing missing translations or a null reference if none of the translations are missing</returns>
        private IEnumerable<CultureInfo> GetMissingTranslations(MarketDescriptionCacheItem item, IEnumerable<CultureInfo> requiredTranslations)
        {
            var requiredTranslationsList = requiredTranslations?.ToList();
            Guard.Argument(requiredTranslationsList, nameof(requiredTranslationsList)).NotNull().NotEmpty();

            if (item == null && requiredTranslationsList != null)
            {
                //we get only those which was not yet fetched
                return requiredTranslationsList.Where(c => !_fetchedLanguages.Contains(c));
            }

            if (requiredTranslationsList != null)
            {
                var missingCultures = requiredTranslationsList.Where(c => !item.HasTranslationsFor(c)).ToList();

                return missingCultures.Any()
                    ? missingCultures
                    : null;
            }

            return new List<CultureInfo>();
        }

        /// <summary>
        /// Merges the provided descriptions with those found in cache
        /// </summary>
        /// <param name="culture">A <see cref="CultureInfo"/> specifying the language of the <code>descriptions</code></param>
        /// <param name="descriptions">A <see cref="IEnumerable{MarketDescriptionDTO}"/> containing market descriptions in specified language</param>
        private void Merge(CultureInfo culture, IEnumerable<MarketDescriptionDTO> descriptions)
        {
            var descriptionList = descriptions?.ToList();
            Guard.Argument(culture, nameof(culture)).NotNull();
            Guard.Argument(descriptionList, nameof(descriptionList)).NotNull().NotEmpty();

            if (descriptionList == null)
            {
                return;
            }

            foreach (var marketDescription in descriptionList)
            {
                var cachedItem = Cache.GetCacheItem(marketDescription.Id.ToString());
                if (cachedItem == null)
                {
                    try
                    {
                        cachedItem = new CacheItem(marketDescription.Id.ToString(), MarketDescriptionCacheItem.Build(marketDescription, culture));
                        Cache.Add(cachedItem, _cacheItemPolicy);
                    }
                    catch (Exception e)
                    {
                        if (!(e is InvalidOperationException))
                        {
                            throw;
                        }

                        CacheLog.LogWarning("Mapping validation for MarketDescriptionCacheItem failed.", e);
                    }
                }
                else
                {
                    ((MarketDescriptionCacheItem) cachedItem.Value).Merge(marketDescription, culture);
                }
            }

            _fetchedLanguages.Add(culture);
        }

        /// <summary>
        /// Asynchronously gets the <see cref="MarketDescriptionCacheItem"/> specified by it's id. If the item is not found in local cache, all items for specified
        /// language are fetched from the service and stored/merged into the local cache.
        /// </summary>
        /// <param name="id">The id of the <see cref="MarketDescriptionCacheItem"/> instance to get</param>
        /// <param name="cultures">A <see cref="IEnumerable{CultureInfo}"/> specifying the languages which the returned item must contain</param>
        /// <returns>A <see cref="Task"/> representing the async operation</returns>
        /// <exception cref="CommunicationException">An error occurred while accessing the remote party</exception>
        /// <exception cref="DeserializationException">An error occurred while deserializing fetched data</exception>
        /// <exception cref="FormatException">An error occurred while mapping deserialized entities</exception>
        private async Task<MarketDescriptionCacheItem> GetMarketInternalAsync(int id, IEnumerable<CultureInfo> cultures)
        { 
            var cultureList = cultures?.ToList();
            Guard.Argument(cultureList, nameof(cultureList)).NotNull().NotEmpty();

            if (!_tokenProvided)
            {
                throw new CommunicationException("Missing AccessToken.", string.Empty, null);
            }

            if (DateTime.Now - TimeOfLastFetch > _fetchInterval)
            {
                _fetchedLanguages.Clear();
                await FetchMarketDescriptionsAsync(cultureList).ConfigureAwait(false);
            }

            // if the market_descriptions was already obtained, there is no need to re-fetch
            // if it is missing, it is MISSING (aka wrong id)
            return GetItemFromCache(id);
        }

        /// <summary>
        /// Asynchronously gets the <see cref="MarketDescriptionCacheItem"/> specified by it's id. If the item is not found in local cache, all items for specified
        /// language are fetched from the service and stored/merged into the local cache.
        /// </summary>
        /// <param name="cultures">A <see cref="IEnumerable{CultureInfo}"/> specifying the languages which the returned item must contain</param>
        /// <returns>A <see cref="Task"/> representing the async operation</returns>
        /// <exception cref="CommunicationException">An error occurred while accessing the remote party</exception>
        /// <exception cref="DeserializationException">An error occurred while deserializing fetched data</exception>
        /// <exception cref="FormatException">An error occurred while mapping deserialized entities</exception>
        private async Task FetchMarketDescriptionsAsync(IEnumerable<CultureInfo> cultures)
        {
            var cultureList = cultures?.ToList();
            Guard.Argument(cultureList, nameof(cultureList)).NotNull().NotEmpty();

            if (!_tokenProvided)
            {
                throw new CommunicationException("Missing AccessToken.", string.Empty, null);
            }

            try
            {
                await _semaphore.WaitAsync();
                var missingLanguages = GetMissingTranslations(null, cultureList).ToList();

                CacheLog.LogInformation($"Fetching MarketDescriptions for languages: [{missingLanguages.Aggregate(",", (s, info) => s + info.TwoLetterISOLanguageName).Remove(0, 1)}].");
                var cultureTaskDictionary = missingLanguages.ToDictionary(l => l, l => _dataProvider.GetDataAsync(l.TwoLetterISOLanguageName));
                await Task.WhenAll(cultureTaskDictionary.Values).ConfigureAwait(false);

                foreach (var cultureTaskPair in cultureTaskDictionary)
                {
                    _metrics.Measure.Counter.Increment(new CounterOptions
                    {
                        Context = "MarketDescriptionCache", MeasurementUnit = Unit.Calls,
                        Name = "FetchMarketDescriptionsAsync"
                    });
                    Merge(cultureTaskPair.Key, cultureTaskPair.Value.Result);
                    CacheLog.LogInformation($"Fetched {cultureTaskPair.Value.Result.Count()} items for {cultureTaskPair.Key.TwoLetterISOLanguageName}.");
                }
                TimeOfLastFetch = DateTime.Now;
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException disposedException)
                {
                    CacheLog.LogWarning($"An error occurred while fetching market descriptions because the object graph is being disposed. Object causing the exception: {disposedException.ObjectName}.");
                }
                throw;
            }
            finally
            {
                if (!_isDisposed)
                {
                    _semaphore.Release();
                }
            }
        }

        private void PauseFetching()
        {
            CacheLog.LogDebug($"Fetching paused for {_minIntervalTimeout.TotalSeconds}s.");
            TimeOfLastFetch = DateTime.Now.AddSeconds(-_fetchInterval.TotalSeconds).AddSeconds(_minIntervalTimeout.TotalSeconds);
        }

        /// <summary>
        /// Disposes un-managed resources associated with the current instance
        /// </summary>
        ~MarketDescriptionCache()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources</param>
        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            if (disposing)
            {
                _semaphore?.Dispose();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the market descriptor.
        /// </summary>
        /// <param name="marketId">The market identifier</param>
        /// <param name="variant">A <see cref="string"/> specifying market variant or a null reference if market is invariant</param>
        /// <param name="cultures">The cultures</param>
        /// <exception cref="CacheItemNotFoundException">The requested key was not found in the cache and could not be loaded</exception>
        public async Task<MarketDescriptionCacheItem> GetMarketDescriptorAsync(int marketId, string variant, IEnumerable<CultureInfo> cultures)
        {
            var cultureList = cultures as List<CultureInfo> ?? cultures.ToList();
            Guard.Argument(marketId, nameof(marketId)).Positive();
            Guard.Argument(cultureList, nameof(cultureList)).NotNull().NotEmpty();

            if (!_tokenProvided)
            {
                throw new CommunicationException("Missing AccessToken.", string.Empty, null);
            }

            MarketDescriptionCacheItem cacheItem;
            try
            {
                cacheItem = await GetMarketInternalAsync(marketId, cultureList).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PauseFetching();
                if (ex is CommunicationException || ex is DeserializationException || ex is MappingException)
                {
                    throw new CacheItemNotFoundException("The requested key was not found in the cache", marketId.ToString(), ex);
                }
                throw;
            }
            if (cacheItem == null)
            {
                PauseFetching();
                throw new CacheItemNotFoundException("The requested key was not found in the cache", marketId.ToString(), null);
            }
            return cacheItem;
        }

        /// <summary>
        /// Registers the health check which will be periodically triggered
        /// </summary>
        public void RegisterHealthCheck()
        {
            //unused
        }

        /// <summary>
        /// Starts the health check and returns <see cref="HealthCheckResult"/>
        /// </summary>
        public HealthCheckResult StartHealthCheck()
        {
            return Cache.Any() ? HealthCheckResult.Healthy($"Cache has {Cache.Count()} items.") : HealthCheckResult.Unhealthy("Cache is empty.");
        }
    }
}