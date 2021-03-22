/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Castle.Core.Internal;
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

        public string GetPathWithReplacements(string path)
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
            if (_uriReplacements.IsNullOrEmpty())
            {
                return await Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.Accepted)).ConfigureAwait(false);
            }

            var path = GetPathWithReplacements(uri.PathAndQuery);
            if(path.IsNullOrEmpty())
            {
                return await Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.BadGateway)).ConfigureAwait(false);
            }

            HttpResponseMessage httpResponseMessage;
            if (File.Exists(path))
            {
                using (StreamReader stream = File.OpenText(path))
                {
                    var cont = stream.ReadToEnd();
                    httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted) {Content = new StringContent(cont)};
                    return await Task.Factory.StartNew(() => httpResponseMessage).ConfigureAwait(false);
                }
            }

            var responseMessage = new HttpResponseMessage(HttpStatusCode.Accepted) {Content = content};
            return await Task.Factory.StartNew(() => responseMessage).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> PostDataAsync(string authorization, Uri uri, HttpContent content = null)
        {
            return await PostDataAsync(uri, content).ConfigureAwait(false);
        }
    }
}