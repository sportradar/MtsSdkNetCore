/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Sportradar.MTS.SDK.Common.Internal.Rest;

// ReSharper disable UnusedMember.Global

namespace Sportradar.MTS.SDK.Test.Helpers
{
    /// <summary>
    /// Data fetcher and poster for testing
    /// </summary>
    public class DataFetcherHelper : IDataFetcher, IDataPoster
    {
        private readonly List<Tuple<string, string>> _uriReplacements;

        public DataFetcherHelper()
        {
        }

        public DataFetcherHelper(IEnumerable<Tuple<string, string>> uriReplacements)
        {
            _uriReplacements = uriReplacements?.ToList();
        }

        private string GetPathWithReplacements(string path)
        {
            return _uriReplacements == null || !_uriReplacements.Any()
                ? path
                : _uriReplacements.Aggregate(path, (current, replacement) => current.Replace(replacement.Item1, replacement.Item2));
        }

        public virtual async Task<Stream> GetDataAsync(Uri uri)
        {
            return await FileHelper.OpenFileAsync(GetPathWithReplacements(uri.PathAndQuery)).ConfigureAwait(false);
        }

        public async Task<Stream> GetDataAsync(string authorization, Uri uri)
        {
            return await GetDataAsync(uri).ConfigureAwait(false);
        }

        public virtual async Task<HttpResponseMessage> PostDataAsync(Uri uri, HttpContent content = null)
        {
            return await Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Accepted)).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> PostDataAsync(string authorization, Uri uri, HttpContent content = null)
        {
            return await PostDataAsync(uri, content).ConfigureAwait(false);
        }
    }
}