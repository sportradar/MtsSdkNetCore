using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sportradar.MTS.SDK.API.Internal;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Cache;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;
using Sportradar.MTS.SDK.Entities.Internal.REST.ReportApiImpl;
using Sportradar.MTS.SDK.Test.Helpers;
using TinyCsvParser;

namespace Sportradar.MTS.SDK.Test
{
    [TestClass]
    [DeploymentItem("TESTDATA/ccf_history_change_client_api-report.csv", "TESTDATA")]
    public class ReportManagerTest
    {
        private const string FileName = "TESTDATA\\ccf_history_change_client_api-report.csv";
        private IDeserializer<market_descriptions> _deserializer;
        private Mock<DataFetcherHelper> _mockDataFetcher;
        private MarketDescriptionsMapperFactory _mapper;
        private IDataProvider<IEnumerable<MarketDescriptionDTO>> _dataProvider;
        private IMarketDescriptionCache _marketDescriptionCache;
        private IEnumerable<CultureInfo> _cultures;

        [TestInitialize]
        public void Init()
        {
            var configInternal = new SdkConfigurationInternal(new SdkConfiguration("username", "password", "host", null, true, "sslServerName",1, 0, 0, "EUR", null, "aaa"), null);

            _cultures = new List<CultureInfo> {new CultureInfo("en")};

            var uri = new Uri(@"https://api.betradar.com/v1/descriptions/en/markets.xml?include_mappings=true");

            _mockDataFetcher = new Mock<DataFetcherHelper>();
            _mockDataFetcher.Setup(p => p.GetDataAsync(It.IsAny<Uri>())).Returns(new DataFetcherHelper(BuilderFactoryHelper.UriReplacements).GetDataAsync(uri));

            _deserializer = new Deserializer<market_descriptions>();
            _mapper = new MarketDescriptionsMapperFactory();

            _dataProvider = new DataProvider<market_descriptions, IEnumerable<MarketDescriptionDTO>>(
                    uri.AbsoluteUri,
                    _mockDataFetcher.Object,
                    _mockDataFetcher.Object,
                    _deserializer,
                    _mapper);

            _marketDescriptionCache = new MarketDescriptionCache(new MemoryCache("InvariantMarketDescriptionCache"),
                _dataProvider,
                _cultures,
                configInternal.AccessToken,
                TimeSpan.FromHours(4),
                new CacheItemPolicy {SlidingExpiration = TimeSpan.FromDays(1)});
        }

        [TestMethod]
        public void NormalDeserializeMarketDescriptionsTest()
        {
            var stream = FileHelper.OpenFile(FileName);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var result = reader.ReadToEnd();
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
            
            var ccfChanges = new List<ICcfChange>();
            try
            {
                var csvParserOptions = new CsvParserOptions(true, ',');
                var csvReaderOptions = new CsvReaderOptions(new []{"\n"});
                var csvParser = new CsvParser<CcfChange>(csvParserOptions, new ReportManager.CsvCcfChangeMapping());
                var records = csvParser.ReadFromString(csvReaderOptions, result);
                Assert.IsNotNull(records);
                Assert.IsTrue(records.Any());

                var minDate = new DateTime(2021, 03, 01);
                var maxDate = new DateTime(2021, 03, 31);
                foreach (var mappingResult in records)
                {
                    if(mappingResult.IsValid)
                    {
                        ValidateCcfChange(mappingResult.Result, minDate, maxDate);
                        ccfChanges.Add(mappingResult.Result);
                    }
                    else
                    {
                        Assert.IsTrue(mappingResult.IsValid, $"Error parsing csv. Index={mappingResult.RowIndex}, Error={mappingResult.Error}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Assert.IsTrue(ccfChanges.Any());
            Assert.AreEqual(6974,ccfChanges.Count);
        }

        //[TestMethod]
        //public void NormalDeserializeMarketDescriptions2Test()
        //{
        //    var stream = FileHelper.OpenFile(FileName);
        //    using var reader = new StreamReader(stream, Encoding.UTF8);
        //    var result = reader.ReadToEnd();
        //    Assert.IsNotNull(result);
        //    Assert.IsFalse(string.IsNullOrEmpty(result));

        //    var ccfChanges = new List<ICcfChange>();

        //    var lines = result.Split("\n");
        //    Assert.IsTrue(lines.Length > 1);
        //    var minDate = new DateTime(2021, 03, 01);
        //    var maxDate = new DateTime(2021, 03, 31);
        //    foreach (var line in lines)
        //    {
        //        var ccfChange = ReportManager.ParseReportLine(line);
        //        if (ccfChange != null)
        //        {
        //            ValidateCcfChange(ccfChange, minDate, maxDate);
        //            ccfChanges.Add(ccfChange);
        //        }
        //    }

        //    Assert.IsTrue(ccfChanges.Any());
        //    Assert.AreEqual(6974,ccfChanges.Count);
        //}

        //[TestMethod]
        //public void NormalDeserializeMarketDescriptions3Test()
        //{
        //    var stream = FileHelper.OpenFile(FileName);
        //    using var reader = new StreamReader(stream, Encoding.UTF8);
        //    var result = reader.ReadToEnd();
        //    Assert.IsNotNull(result);
        //    Assert.IsFalse(string.IsNullOrEmpty(result));

        //    var ccfChanges = new List<ICcfChange>();

        //    var lines = result.Split("\n");
        //    Assert.IsTrue(lines.Length > 1);
        //    var minDate = new DateTime(2021, 03, 01);
        //    var maxDate = new DateTime(2021, 03, 31);
        //    foreach (var line in lines)
        //    {
        //        var ccfChange = ReportManager.ParseReportLine(line);
        //        if (ccfChange != null)
        //        {
        //            ValidateCcfChange(ccfChange, minDate, maxDate);
        //            ccfChanges.Add(ccfChange);
        //        }
        //    }

        //    Assert.IsTrue(ccfChanges.Any());
        //    Assert.AreEqual(6974,ccfChanges.Count);
        //}

        private void ValidateCcfChange(ICcfChange ccfChange, DateTime minDate, DateTime maxDate)
        {
            Assert.IsNotNull(ccfChange);
            Assert.IsTrue(ccfChange.Timestamp > minDate);
            Assert.IsTrue(ccfChange.Timestamp < maxDate);
            Assert.IsTrue(ccfChange.BookmakerId > 0);
            Assert.IsTrue(ccfChange.SubBookmakerId > 0);
            Assert.IsTrue(!string.IsNullOrEmpty(ccfChange.SourceId));
        }
    }
}
