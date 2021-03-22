/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.API.Internal;
using Sportradar.MTS.SDK.API.Internal.MtsAuth;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.REST.ReportApiImpl;
using Sportradar.MTS.SDK.Test.Helpers;
using TinyCsvParser;

namespace Sportradar.MTS.SDK.Test.Api
{
    [TestClass]
    [DeploymentItem("TESTDATA/ccf_history_change_client_api-report.csv", "TESTDATA")]
    public class ReportManagerTests
    {
        private const string FileName = "TESTDATA\\ccf_history_change_client_api-report.csv";
        private Mock<DataFetcherHelper> _mockDataFetcher;
        private IMtsAuthService _mtsAuthService;
        private IReportManager _reportManager;

        [TestInitialize]
        public void Init()
        {
            var config = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("mtsgate-ci.betradar.com")
                .SetVirtualHost("/vhost")
                .SetLimitId(111)
                .SetBookmakerId(333)
                .SetAccessToken("erErk34kfErr")
                .SetCurrency("EUR")
                .SetNode(10)
                .SetMtsClientApiHost("https://mts-api-ci.betradar.com/edge/proxy")
                .SetKeycloakHost("https://mts-auth-ci.betradar.com")
                .SetKeycloakUsername("keycloackUsername")
                .SetKeycloakPassword("keycloackPassword")
                .SetKeycloakSecret("53d342-4a7c-dgdbd23-9e1b-93822f2")
                .Build();

            InitReportManager(config);
        }

        private void InitReportManager(ISdkConfiguration config = null)
        {
            var newConfig = config ?? MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("mtsgate-ci.betradar.com")
                .SetVirtualHost("/vhost")
                .SetLimitId(111)
                .SetBookmakerId(333)
                .SetAccessToken("erErk34kfErr")
                .SetCurrency("EUR")
                .SetNode(10)
                .SetMtsClientApiHost("https://mts-api-ci.betradar.com/edge/proxy")
                .SetKeycloakHost("https://mts-auth-ci.betradar.com")
                .SetKeycloakUsername("keycloackUsername")
                .SetKeycloakPassword("keycloackPassword")
                .SetKeycloakSecret("53d342-4a7c-dgdbd23-9e1b-93822f2")
                .Build();

            _mtsAuthService = new MtsAuthServiceHelper(config);

            var uri = new Uri(newConfig.KeycloakHost + "/ReportingCcf/external/api/report/export/history/ccf/changes/client/api");

            _mockDataFetcher = new Mock<DataFetcherHelper>();
            _mockDataFetcher.Setup(p => p.GetDataAsync(It.IsAny<Uri>())).Returns(new DataFetcherHelper(BuilderFactoryHelper.UriReplacements).GetDataAsync(uri));

            _reportManager = new ReportManager(_mockDataFetcher.Object, uri.OriginalString, _mtsAuthService, null, newConfig);
        }

        [TestMethod]
        public void CheckCsvResultParsingTest()
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
                var records = csvParser.ReadFromString(csvReaderOptions, result).ToList();
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

        [TestMethod]
        public void CheckCsvFileResultParsingTest()
        {
            var minDate = new DateTime(2021, 03, 01);
            var maxDate = new DateTime(2021, 03, 22);
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate).Result;
            Assert.IsNotNull(ccfChanges);
            Assert.IsTrue(ccfChanges.Any());
            
            foreach (var ccfChange in ccfChanges)
            {
                ValidateCcfChange(ccfChange, minDate, maxDate);
            }
            Assert.AreEqual(6974,ccfChanges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CheckStartDateWrongTest()
        {
            var minDate = DateTime.Now.AddDays(1);
            var maxDate = new DateTime(2021, 03, 22);
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate).Result;
            Assert.IsNull(ccfChanges);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CheckStartDateBeforeEndDateTest()
        {
            var minDate = DateTime.Now.AddDays(-14);
            var maxDate = minDate.AddDays(-1);
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate).Result;
            Assert.IsNull(ccfChanges);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CheckBookmakerIdMissingTest()
        {
            var newConfig = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("mtsgate-ci.betradar.com")
                .SetVirtualHost("/vhost")
                .SetLimitId(111)
                //.SetBookmakerId(333)
                .SetAccessToken("erErk34kfErr")
                .SetCurrency("EUR")
                .SetNode(10)
                .SetMtsClientApiHost("https://mts-api-ci.betradar.com/edge/proxy")
                .SetKeycloakHost("https://mts-auth-ci.betradar.com")
                .SetKeycloakUsername("keycloackUsername")
                .SetKeycloakPassword("keycloackPassword")
                .SetKeycloakSecret("53d342-4a7c-dgdbd23-9e1b-93822f2")
                .Build();

            InitReportManager(newConfig);
            var minDate = DateTime.Now.AddDays(-14);
            var maxDate = DateTime.Now;
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate).Result;
            Assert.IsNull(ccfChanges);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CheckKeycloakUsernameMissingTest()
        {
            var newConfig = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("mtsgate-ci.betradar.com")
                .SetVirtualHost("/vhost")
                .SetLimitId(111)
                .SetBookmakerId(333)
                .SetAccessToken("erErk34kfErr")
                .SetCurrency("EUR")
                .SetNode(10)
                .SetMtsClientApiHost("https://mts-api-ci.betradar.com/edge/proxy")
                .SetKeycloakHost("https://mts-auth-ci.betradar.com")
                //.SetKeycloakUsername("keycloackUsername")
                .SetKeycloakPassword("keycloackPassword")
                .SetKeycloakSecret("53d342-4a7c-dgdbd23-9e1b-93822f2")
                .Build();

            InitReportManager(newConfig);
            var minDate = DateTime.Now.AddDays(-14);
            var maxDate = DateTime.Now;
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate).Result;
            Assert.IsNull(ccfChanges);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CheckKeycloakPasswordMissingTest()
        {
            var newConfig = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("mtsgate-ci.betradar.com")
                .SetVirtualHost("/vhost")
                .SetLimitId(111)
                .SetBookmakerId(333)
                .SetAccessToken("erErk34kfErr")
                .SetCurrency("EUR")
                .SetNode(10)
                .SetMtsClientApiHost("https://mts-api-ci.betradar.com/edge/proxy")
                .SetKeycloakHost("https://mts-auth-ci.betradar.com")
                .SetKeycloakUsername("keycloackUsername")
                //.SetKeycloakPassword("keycloackPassword")
                .SetKeycloakSecret("53d342-4a7c-dgdbd23-9e1b-93822f2")
                .Build();

            InitReportManager(newConfig);
            var minDate = DateTime.Now.AddDays(-14);
            var maxDate = DateTime.Now;
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate).Result;
            Assert.IsNull(ccfChanges);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CheckKeycloakSecretMissingTest()
        {
            var newConfig = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("mtsgate-ci.betradar.com")
                .SetVirtualHost("/vhost")
                .SetLimitId(111)
                .SetBookmakerId(333)
                .SetAccessToken("erErk34kfErr")
                .SetCurrency("EUR")
                .SetNode(10)
                .SetMtsClientApiHost("https://mts-api-ci.betradar.com/edge/proxy")
                .SetKeycloakHost("https://mts-auth-ci.betradar.com")
                .SetKeycloakUsername("keycloackUsername")
                .SetKeycloakPassword("keycloackPassword")
                //.SetKeycloakSecret("53d342-4a7c-dgdbd23-9e1b-93822f2")
                .Build();

            InitReportManager(newConfig);
            var minDate = DateTime.Now.AddDays(-14);
            var maxDate = DateTime.Now;
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate).Result;
            Assert.IsNull(ccfChanges);
        }

        [TestMethod]
        public void CheckMinimalConfigTest()
        {
            var newConfig = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("mtsgate-ci.betradar.com")
                .SetVirtualHost("/vhost")
                .SetLimitId(111)
                //.SetBookmakerId(333)
                .SetAccessToken("erErk34kfErr")
                .SetCurrency("EUR")
                .SetNode(10)
                .SetMtsClientApiHost("https://mts-api-ci.betradar.com/edge/proxy")
                .SetKeycloakHost("https://mts-auth-ci.betradar.com")
                //.SetKeycloakUsername("keycloackUsername")
                //.SetKeycloakPassword("keycloackPassword")
                .SetKeycloakSecret("53d342-4a7c-dgdbd23-9e1b-93822f2")
                .Build();

            InitReportManager(newConfig);
            var minDate = DateTime.Now.AddDays(-14);
            var maxDate = DateTime.Now;
            var ccfChanges = _reportManager.GetHistoryCcfChangeCsvExportAsync(minDate, maxDate, 1, null, null, SourceType.Customer, "keyUsername", "keyPassword").Result;
            Assert.IsNotNull(ccfChanges);
            Assert.AreEqual(6974,ccfChanges.Count);
        }

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
