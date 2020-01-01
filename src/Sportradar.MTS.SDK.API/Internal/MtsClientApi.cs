/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;
using System.Runtime.Caching;
using App.Metrics;
using App.Metrics.Counter;
using Sportradar.MTS.SDK.Common;

namespace Sportradar.MTS.SDK.API.Internal
{
    /// <summary>
    /// A <see cref="IMtsClientApi"/> implementation acting as an entry point to the MTS Client API
    /// </summary>
    public class MtsClientApi : IMtsClientApi
    {
        /// <summary>
        /// A log4net.ILog instance used for logging execution logs
        /// </summary>
        private static readonly ILogger ExecutionLog = SdkLoggerFactory.GetLoggerForRestTraffic(typeof(MtsClientApi));

        /// <summary>
        /// A log4net.ILog instance used for logging client iteration logs
        /// </summary>
        private static readonly ILogger InteractionLog = SdkLoggerFactory.GetLoggerForClientInteraction(typeof(MtsClientApi));

        /// <summary>
        /// The <see cref="IDataProvider{MaxStakeImpl}"/> for getting max stake
        /// </summary>
        private readonly IDataProvider<MaxStakeImpl> _maxStakeDataProvider;

        /// <summary>
        /// The <see cref="IDataProvider{CcfImpl}"/> for getting ccf
        /// </summary>
        private readonly IDataProvider<CcfImpl> _ccfDataProvider;

        /// <summary>
        /// The <see cref="IDataProvider{KeycloakAuthorization}"/> for getting authorization token
        /// </summary>
        private readonly IDataProvider<KeycloakAuthorization> _authorizationDataProvider;

        /// <summary>
        /// Username used for getting authorization token
        /// </summary>
        private readonly string _username;

        /// <summary>
        /// Password used for getting authorization token
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Secret used for getting authorization token
        /// </summary>
        private readonly string _secret;

        /// <summary>
        /// Cache for storing authorization tokens
        /// </summary>
        private readonly ObjectCache _tokenCache = new MemoryCache("tokenCache");

        /// <summary>
        /// Lock for synchronizing access to token cache
        /// </summary>
        private readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1, 1);

        private readonly IMetricsRoot _metrics;

        public MtsClientApi(IDataProvider<MaxStakeImpl> maxStakeDataProvider, 
                            IDataProvider<CcfImpl> ccfDataProvider, 
                            IDataProvider<KeycloakAuthorization> authorizationDataProvider, 
                            string username, 
                            string password, 
                            string secret, 
                            IMetricsRoot metrics)
        {
            Guard.Argument(maxStakeDataProvider, nameof(maxStakeDataProvider)).NotNull();
            Guard.Argument(ccfDataProvider, nameof(ccfDataProvider)).NotNull();
            Guard.Argument(authorizationDataProvider, nameof(authorizationDataProvider)).NotNull();

            _maxStakeDataProvider = maxStakeDataProvider;
            _ccfDataProvider = ccfDataProvider;
            _authorizationDataProvider = authorizationDataProvider;
            _username = username;
            _password = password;
            _secret = secret;
            _metrics = metrics ?? SdkMetricsFactory.MetricsRoot;
        }

        public async Task<long> GetMaxStakeAsync(ITicket ticket)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();

            return await GetMaxStakeAsync(ticket, _username, _password).ConfigureAwait(false);
        }

        public async Task<long> GetMaxStakeAsync(ITicket ticket, string username, string password)
        {
            Guard.Argument(ticket, nameof(ticket)).NotNull();
            Guard.Argument(username, nameof(username)).NotNull().NotEmpty();
            Guard.Argument(password, nameof(password)).NotNull().NotEmpty();

            _metrics.Measure.Counter.Increment(new CounterOptions{ Context="MtsClientApi", Name="GetMaxStakeAsync", MeasurementUnit = Unit.Calls});
            InteractionLog.LogInformation($"Called GetMaxStakeAsync with ticketId={ticket.TicketId}.");

            try
            {
                var token = await GetTokenAsync(username, password).ConfigureAwait(false);
                var content = new StringContent(ticket.ToJson(), Encoding.UTF8, "application/json");
                var maxStake = await _maxStakeDataProvider.PostDataAsync(token, content, new[] { "" }).ConfigureAwait(false);
                if (maxStake == null)
                {
                    throw new Exception("Failed to get max stake.");
                }
                return maxStake.MaxStake;
            }
            catch (Exception e)
            {
                ExecutionLog.LogError(e.Message, e);
                ExecutionLog.LogWarning($"Getting max stake for ticketId={ticket.TicketId} failed.");
                throw;
            }
        }

        public async Task<ICcf> GetCcfAsync(string sourceId)
        {
            Guard.Argument(sourceId, nameof(sourceId)).NotNull();

            return await GetCcfAsync(sourceId, _username, _password).ConfigureAwait(false);
        }

        public async Task<ICcf> GetCcfAsync(string sourceId, string username, string password)
        {
            Guard.Argument(sourceId, nameof(sourceId)).NotNull();
            Guard.Argument(username, nameof(username)).NotNull().NotEmpty();
            Guard.Argument(password, nameof(password)).NotNull().NotEmpty();

            _metrics.Measure.Counter.Increment(new CounterOptions { Context = "MtsClientApi", Name = "GetCcfAsync", MeasurementUnit = Unit.Calls });
            InteractionLog.LogInformation($"Called GetCcfAsync with sourceId={sourceId}.");

            try
            {
                var token = await GetTokenAsync(username, password).ConfigureAwait(false);
                return await _ccfDataProvider.GetDataAsync(token, new[] { sourceId }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                ExecutionLog.LogError(e.Message, e);
                ExecutionLog.LogWarning($"Getting ccf for sourceId={sourceId} failed.");
                throw;
            }
        }

        private async Task<string> GetTokenAsync(string username, string password)
        {
            var cacheKey = $"{_secret}:{username}:{password}";
            var ci = _tokenCache.GetCacheItem(cacheKey);
            if (ci?.Value != null)
                return (string) ci.Value;

            try
            {
                await _tokenSemaphore.WaitAsync();
                ci = _tokenCache.GetCacheItem(cacheKey);
                if (ci?.Value != null)
                {
                    return (string)ci.Value;
                }

                var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", "mts-edge-ext"),
                        new KeyValuePair<string, string>("client_secret", _secret),
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", password),
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("client_auth_method", "client-secret")
                    });
                try
                {
                    var authorization = await _authorizationDataProvider.PostDataAsync(content, new[] { "" }).ConfigureAwait(false);
                    _tokenCache.Add(cacheKey, authorization.AccessToken, authorization.Expires.AddSeconds(-30));
                    return authorization.AccessToken;
                }
                catch (Exception e)
                {
                    ExecutionLog.LogError(e.Message, e);
                    ExecutionLog.LogWarning("Error getting token from authorization server.");
                    throw;
                }
            }
            finally
            {
                _tokenSemaphore.Release();
            }
        }
    }
}
