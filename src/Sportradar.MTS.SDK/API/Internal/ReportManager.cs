/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Castle.Core.Internal;
using Dawn;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.API.Internal.MtsAuth;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Internal.Rest;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.REST.ReportApiImpl;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace Sportradar.MTS.SDK.API.Internal
{
    internal class ReportManager : IReportManager
    {
        /// <summary>
        /// A logger instance used for logging execution logs
        /// </summary>
        private static readonly ILogger ExecutionLog = SdkLoggerFactory.GetLoggerForExecution(typeof(MtsClientApi));

        /// <summary>
        /// A logger instance used for logging client iteration logs
        /// </summary>
        private static readonly ILogger InteractionLog = SdkLoggerFactory.GetLoggerForClientInteraction(typeof(MtsClientApi));

        /// <summary>
        /// The timestamp format
        /// </summary>
        private const string TimestampFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// The <see cref="IDataProvider{T}"/> for ccf change history
        /// </summary>
        private readonly IDataFetcher _ccfChangeHistoryFetcher;

        /// <summary>
        /// The CCF change history URI
        /// </summary>
        private readonly string _ccfChangeHistoryUri;

        /// <summary>
        /// The MTS authentication service
        /// </summary>
        private readonly IMtsAuthService _mtsAuthService;

        /// <summary>
        /// The configuration
        /// </summary>
        private readonly ISdkConfiguration _config;

        /// <summary>
        /// The metrics
        /// </summary>
        private readonly IMetricsRoot _metrics;

        public ReportManager(IDataFetcher ccfChangeHistoryFetcher,
                             string ccfChangeHistoryUri,
                             IMtsAuthService mtsAuthService,
                             IMetricsRoot metrics,
                             ISdkConfiguration config)
        {
            Guard.Argument(ccfChangeHistoryFetcher, nameof(ccfChangeHistoryFetcher)).NotNull();
            Guard.Argument(ccfChangeHistoryUri, nameof(ccfChangeHistoryUri)).NotNull().NotEmpty();
            Guard.Argument(config, nameof(config)).NotNull();
            
            _ccfChangeHistoryFetcher = ccfChangeHistoryFetcher;
            _ccfChangeHistoryUri = ccfChangeHistoryUri;
            _mtsAuthService = mtsAuthService;
            _metrics = metrics ?? SdkMetricsFactory.MetricsRoot;
            _config = config;
        }

        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Needs more arguments")]
        public async Task GetHistoryCcfChangeCsvExportAsync(Stream outputStream, 
                                                            DateTime startDate, 
                                                            DateTime endDate, 
                                                            int? bookmakerId = null,
                                                            List<int> subBookmakerIds = null, 
                                                            string sourceId = null, 
                                                            SourceType sourceType = SourceType.Customer, 
                                                            string username = null, 
                                                            string password = null)
        {
            CheckArguments(outputStream, startDate, endDate, bookmakerId, username, password);

            var result = await GetHistoryCcfChangeAsync(startDate, endDate, bookmakerId, subBookmakerIds, sourceId, sourceType, username, password).ConfigureAwait(false);
            await result.CopyToAsync(outputStream).ConfigureAwait(false);
        }
        
        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Needs more arguments")]
        public async Task<List<ICcfChange>> GetHistoryCcfChangeCsvExportAsync(DateTime startDate, 
                                                                              DateTime endDate, 
                                                                              int? bookmakerId = null,
                                                                              List<int> subBookmakerIds = null, 
                                                                              string sourceId = null, 
                                                                              SourceType sourceType = SourceType.Customer, 
                                                                              string username = null, 
                                                                              string password = null)
        {
            CheckArguments(startDate, endDate, bookmakerId, username, password);

            var resultStream = await GetHistoryCcfChangeAsync(startDate, endDate, bookmakerId, subBookmakerIds, sourceId, sourceType, username, password).ConfigureAwait(false);

            var ccfChanges = new List<ICcfChange>();

            try
            {
                var csvParserOptions = new CsvParserOptions(true, ',');
                var csvParser = new CsvParser<CcfChange>(csvParserOptions, new CsvCcfChangeMapping());
                var records = csvParser.ReadFromStream(resultStream, Encoding.UTF8, true);
                foreach (var mappingResult in records)
                {
                    if(mappingResult.IsValid)
                    {
                        ccfChanges.Add(mappingResult.Result);
                    }
                    else
                    {
                        ExecutionLog.LogWarning($"Error parsing CSV result. Index={mappingResult.RowIndex}, Error={mappingResult.Error}");
                    }
                }
            }
            catch (Exception e)
            {
                ExecutionLog.LogError($"Parsing results from GetHistoryCcfChangeAsync failed. Error={e.Message}");
            }

            return ccfChanges;
        }

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
        private void CheckArguments(Stream outputStream,
                                    DateTime startDate,
                                    DateTime endDate,
                                    int? bookmakerId = null,
                                    string username = null,
                                    string password = null)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream), "Missing outputStream argument");
            }

            CheckArguments(startDate, endDate, bookmakerId, username, password);
        }

        // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
        private void CheckArguments(DateTime startDate,
                                    DateTime endDate,
                                    int? bookmakerId = null,
                                    string username = null,
                                    string password = null)
        {
            if (startDate.Equals(DateTime.MinValue) || startDate.Equals(DateTime.Now) || startDate > DateTime.Now)
            {
                throw new ArgumentException("StartDate must be set in the past (within last 30 days)", nameof(startDate));
            }

            if (endDate.Equals(DateTime.MinValue) || endDate > DateTime.Now)
            {
                throw new ArgumentException("EndDate must be set in the past (within last 30 days)", nameof(endDate));
            }

            if (endDate < startDate)
            {
                throw new ArgumentException("EndDate must greater then StartDate", nameof(endDate));
            }

            if (bookmakerId == null && _config.BookmakerId == 0)
            {
                throw new ArgumentException("Missing BookmakerId. Must be provided in method or set via configuration", nameof(bookmakerId));
            }

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(_config.KeycloakUsername))
            {
                throw new ArgumentException("Missing Username. Must be provided in method or set via configuration (KeycloackUsername)", nameof(username));
            }

            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(_config.KeycloakPassword))
            {
                throw new ArgumentException("Missing Password. Must be provided in method or set via configuration (KeycloackPassword)", nameof(password));
            }
        }

        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Needs 8 arguments")]
        private async Task<Stream> GetHistoryCcfChangeAsync(DateTime startDate,
                                                            DateTime endDate, 
                                                            int? bookmakerId = null,
                                                            IReadOnlyCollection<int> subBookmakerIds = null, 
                                                            string sourceId = null, 
                                                            SourceType sourceType = SourceType.Customer, 
                                                            string username = null, 
                                                            string password = null)
        {
            _metrics.Measure.Counter.Increment(new CounterOptions { Context = "ReportManager", Name = "GetHistoryCcfChangeAsync", MeasurementUnit = Unit.Calls });
            InteractionLog.LogInformation($"Called GetHistoryCcfChangeAsync for period: {startDate} - {endDate}");

            var fullUri = new Uri(_ccfChangeHistoryUri);
            try
            {
                var token = await _mtsAuthService.GetTokenAsync(username, password).ConfigureAwait(false);
                
                fullUri = GenerateFullUri(startDate, endDate, bookmakerId, subBookmakerIds, sourceId, sourceType);
                var resultStream = await _ccfChangeHistoryFetcher.GetDataAsync(token, fullUri).ConfigureAwait(false);
                
                return resultStream;
            }
            catch (Exception e)
            {
                ExecutionLog.LogError(e.Message, e);
                ExecutionLog.LogWarning($"Getting ccf changes from url={fullUri} failed.");
                throw;
            }
        }

        private Uri GenerateFullUri(DateTime startDate, 
                                    DateTime endDate, 
                                    int? bookmakerId = null,
                                    IReadOnlyCollection<int> subBookmakerIds = null, 
                                    string sourceId = null, 
                                    SourceType sourceType = SourceType.Customer)
        {
            bookmakerId ??= _config.BookmakerId;
            var filter = $"?startDatetime={startDate.ToString(TimestampFormat)}&endDatetime={endDate.ToString(TimestampFormat)}&bookmakerId={bookmakerId}";
            if(!subBookmakerIds.IsNullOrEmpty())
            {
                filter += $"&subBookmakerId={string.Join(",", subBookmakerIds!)}";
            }
            if(!string.IsNullOrEmpty(sourceId))
            {
                filter += $"&sourceId={sourceId}";
            }
            filter += $"&sourceType={sourceType}";
            
            var fullUri = new Uri(_ccfChangeHistoryUri + filter);
            return fullUri;
        }

        internal class CsvCcfChangeMapping : CsvMapping<CcfChange>
        {
            public CsvCcfChangeMapping()
            {
                MapProperty(0, x => x.Timestamp, new DateTimeTimestampTypeConverter());
                MapProperty(1, x => x.BookmakerId);
                MapProperty(2, x => x.SubBookmakerId);
                MapProperty(3, x => x.SourceId);
                MapProperty(4, x => x.SourceType, new EnumConverter<SourceType>(true));
                MapProperty(5, x => x.Ccf);
                MapProperty(6, x => x.PreviousCcf);
                MapProperty(7, x => x.SportId);
                MapProperty(8, x => x.SportName);
                MapProperty(9, x => x.IsLive);
            }
        }

        private class DateTimeTimestampTypeConverter : ITypeConverter<DateTime>
        {
            public Type TargetType => typeof(DateTime);

            public bool TryConvert(string value, out DateTime result)
            {
                return DateTime.TryParseExact(value, TimestampFormat, null, DateTimeStyles.AssumeUniversal, out result);
            }
        }
    }
}
