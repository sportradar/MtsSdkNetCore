/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Sportradar.MTS.SDK.Common.Exceptions;

namespace Sportradar.MTS.SDK.Common.Internal.Rest
{
    /// <summary>
    /// A <see cref="IDataFetcher" /> which uses the HTTP requests to fetch the requested data
    /// </summary>
    /// <seealso cref="MarshalByRefObject" />
    /// <seealso cref="IDataFetcher" />
    /// <seealso cref="IDataPoster" />
    internal class HttpDataFetcher : MarshalByRefObject, IDataFetcher, IDataPoster
    {
        /// <summary>
        /// A <see cref="HttpClient"/> used to invoke HTTP requests
        /// </summary>
        private readonly HttpClient _client;

        private readonly int _connectionFailureLimit;

        private readonly int _connectionFailureTimeBetweenNewRequestsInSec;

        private int _connectionFailureCount;

        private long _timeOfLastFailure;

        private bool _blockingModeActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpDataFetcher"/> class
        /// </summary>
        /// <param name="client">A <see cref="HttpClient"/> used to invoke HTTP requests</param>
        /// <param name="accessToken">A token used when making the http requests</param>
        /// <param name="connectionFailureLimit">Indicates the limit of consecutive request failures, after which it goes in "blocking mode"</param>
        /// <param name="connectionFailureTimeout">indicates the timeout after which comes out of "blocking mode" (in seconds)</param>
        public HttpDataFetcher(HttpClient client, string accessToken, int connectionFailureLimit = 5, int connectionFailureTimeout = 15)
        {
            Guard.Argument(client, nameof(client)).NotNull();
            Guard.Argument(client.DefaultRequestHeaders, nameof(client.DefaultRequestHeaders)).NotNull();
            Guard.Argument(connectionFailureLimit, nameof(connectionFailureLimit)).Positive();
            Guard.Argument(connectionFailureTimeout, nameof(connectionFailureTimeout)).Positive();

            _client = client;
            if (!string.IsNullOrEmpty(accessToken) && !_client.DefaultRequestHeaders.Contains("x-access-token"))
            {
                _client.DefaultRequestHeaders.Add("x-access-token", accessToken);
            }

            _connectionFailureLimit = connectionFailureLimit;
            _connectionFailureTimeBetweenNewRequestsInSec = connectionFailureTimeout;
            _blockingModeActive = false;
        }

        /// <summary>
        /// Asynchronously gets a <see cref="Stream" /> containing data fetched from the provided <see cref="Uri" />
        /// </summary>
        /// <param name="uri">The <see cref="Uri" /> of the resource to be fetched</param>
        /// <returns>A <see cref="Task" /> which, when completed will return a <see cref="Stream" /> containing fetched data</returns>
        /// <exception cref="CommunicationException">Failed to execute http get</exception>
        public virtual async Task<Stream> GetDataAsync(Uri uri)
        {
            return await GetDataAsync(null, uri).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously gets a <see cref="Stream" /> containing data fetched from the provided <see cref="Uri" />
        /// </summary>
        /// <param name="authorization">The value of authorization header</param>
        /// <param name="uri">The <see cref="Uri" /> of the resource to be fetched</param>
        /// <returns>A <see cref="Task" /> which, when completed will return a <see cref="Stream" /> containing fetched data</returns>
        /// <exception cref="CommunicationException">Failed to execute http get</exception>
        public virtual async Task<Stream> GetDataAsync(string authorization, Uri uri)
        {
            ValidateConnection(uri);
            var responseMessage = new HttpResponseMessage();
            try
            {
                _client.DefaultRequestHeaders.Authorization = authorization != null
                    ? new AuthenticationHeaderValue("Bearer", authorization)
                    : null;
                responseMessage = await _client.GetAsync(uri).ConfigureAwait(false);
                RecordSuccess();
                if (!responseMessage.IsSuccessStatusCode)
                {
                    throw new CommunicationException($"Response StatusCode={responseMessage.StatusCode} does not indicate success.", uri.ToString(), responseMessage.StatusCode, null);
                }
                return await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException)
                {
                    RecordFailure();
                    throw new CommunicationException("Failed to execute http get", uri.ToString(), responseMessage.StatusCode, ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Asynchronously gets a <see cref="Stream" /> containing data fetched from the provided <see cref="Uri" />
        /// </summary>
        /// <param name="uri">The <see cref="Uri" /> of the resource to be fetched</param>
        /// <param name="content">A <see cref="HttpContent" /> to be posted to the specific <see cref="Uri" /></param>
        /// <returns>A <see cref="Task" /> which, when successfully completed will return a <see cref="HttpResponseMessage" /></returns>
        /// <exception cref="CommunicationException">Failed to execute http post</exception>
        public virtual async Task<HttpResponseMessage> PostDataAsync(Uri uri, HttpContent content = null)
        {
            return await PostDataAsync(null, uri, content).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously gets a <see cref="Stream" /> containing data fetched from the provided <see cref="Uri" />
        /// </summary>
        /// <param name="authorization">The value of authorization header</param>
        /// <param name="uri">The <see cref="Uri" /> of the resource to be fetched</param>
        /// <param name="content">A <see cref="HttpContent" /> to be posted to the specific <see cref="Uri" /></param>
        /// <returns>A <see cref="Task" /> which, when successfully completed will return a <see cref="HttpResponseMessage" /></returns>
        /// <exception cref="CommunicationException">Failed to execute http post</exception>
        public virtual async Task<HttpResponseMessage> PostDataAsync(string authorization, Uri uri, HttpContent content = null)
        {
            ValidateConnection(uri);
            var responseMessage = new HttpResponseMessage();
            try
            {
                _client.DefaultRequestHeaders.Authorization = authorization != null
                    ? new AuthenticationHeaderValue("Bearer", authorization)
                    : null;
                responseMessage = await _client.PostAsync(uri, content ?? new StringContent(string.Empty)).ConfigureAwait(false);
                RecordSuccess();
                return responseMessage;
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException)
                {
                    RecordFailure();
                    throw new CommunicationException("Failed to execute http post", uri.ToString(), responseMessage.StatusCode, ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Records that the request made was successful
        /// </summary>
        protected void RecordSuccess()
        {
            Interlocked.Exchange(ref _connectionFailureCount, 0);
            Interlocked.Exchange(ref _timeOfLastFailure, DateTime.MinValue.Ticks);
            _blockingModeActive = false;
        }

        /// <summary>
        /// Records that the request ended with HttpRequestException or was taking too long and was canceled
        /// </summary>
        protected void RecordFailure()
        {
            Interlocked.Increment(ref _connectionFailureCount);
            Interlocked.Exchange(ref _timeOfLastFailure, DateTime.Now.Ticks);
        }

        /// <summary>
        /// Validates if the request should be made or too many errors happens and should be omitted
        /// </summary>
        /// <param name="uri">The URI of the request to be made</param>
        /// <exception cref="CommunicationException">Failed to execute request due to previous failures</exception>
        protected void ValidateConnection(Uri uri)
        {
            var timeOfLastFailure = new DateTime(_timeOfLastFailure);
            var resetTime = DateTime.Now.AddSeconds(-_connectionFailureTimeBetweenNewRequestsInSec);
            if (timeOfLastFailure < resetTime && _blockingModeActive)
            {
                RecordSuccess();
                return;
            }

            if (_connectionFailureCount >= _connectionFailureLimit && !_blockingModeActive)
            {
                _blockingModeActive = true;
            }

            if (_blockingModeActive)
            {
                throw new CommunicationException("Failed to execute request due to previous failures.", uri.ToString(), null);
            }
        }
    }
}
