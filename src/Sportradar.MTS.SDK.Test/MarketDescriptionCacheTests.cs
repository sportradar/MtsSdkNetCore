/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Cache;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;
using Sportradar.MTS.SDK.Test.Helpers;

namespace Sportradar.MTS.SDK.Test
{
    [TestClass]
    [DeploymentItem("XML/market_descriptions.en.xml", "XML")]
    public class MarketDescriptionCacheTests
    {
        private const string FileName = "market_descriptions.en.xml";
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

            var uri = new Uri(@"https://global.api.betradar.com/v1/descriptions/en/markets.xml?include_mappings=true");

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
                configInternal.AccessToken,
                TimeSpan.FromHours(4),
                new CacheItemPolicy {SlidingExpiration = TimeSpan.FromDays(1)});
        }

        [TestMethod]
        public void NormalDeserializeMarketDescriptionsTest()
        {
            var stream = FileHelper.OpenFile(FileName);
            var deserializedObject = _deserializer.Deserialize(stream);
            var data = _mapper.CreateMapper(deserializedObject).Map();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());
        }

        [TestMethod]
        public void PlainDeserializeTest()
        {
            var xmlStream = new StreamReader(FileHelper.OpenFile("XML", FileName));
            var serializer = new XmlSerializer(typeof(market_descriptions));
            var result = (market_descriptions)serializer.Deserialize(xmlStream);
            var data = _mapper.CreateMapper(result).Map();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());
        }

        [TestMethod]
        public void CacheAutoLoadTest()
        {
            var mdCache = (MarketDescriptionCache) _marketDescriptionCache;
            var marketDescriptor = mdCache.GetMarketDescriptorAsync(1, null, _cultures);
            var i = 50;
            while (mdCache.TimeOfLastFetch == DateTime.MinValue  && i > 0)
            {
                Thread.Sleep(50);
                i--;
            }
            Assert.IsTrue(mdCache.Cache.GetCount() > 0);
            Assert.IsNotNull(marketDescriptor);
        }

        [TestMethod]
        public void DataFetcherHelperTest()
        {
            var uri = new Uri(@"https://global.api.betradar.com/" + FileName);
            var file = _mockDataFetcher.Object.GetDataAsync(uri).Result;

            Assert.IsNotNull(file);
        }

        [TestMethod]
        public void MarketDescriptionIsObtainedTest()
        {
            const int marketId = 447;

            var mdCache = (MarketDescriptionCache)_marketDescriptionCache;
            var marketDescription = _marketDescriptionCache.GetMarketDescriptorAsync(marketId, null, _cultures).Result;

            Assert.IsTrue(mdCache.Cache.GetCount() > 0);
            Assert.IsNotNull(marketDescription);
            Assert.AreEqual(marketId, marketDescription.Id);
        }

        [TestMethod]
        public void MarketDescriptionsAreFetchedOnlyOnceTest()
        {
            const int marketId = 447;

            var mdCache = (MarketDescriptionCache)_marketDescriptionCache;
            var marketDescription1 = _marketDescriptionCache.GetMarketDescriptorAsync(62, null, _cultures).Result;
            var marketDescription2 = _marketDescriptionCache.GetMarketDescriptorAsync(350, null, _cultures).Result;
            var marketDescription3 = _marketDescriptionCache.GetMarketDescriptorAsync(marketId, null, _cultures).Result;

            Assert.IsTrue(mdCache.Cache.GetCount() > 0);
            Assert.IsNotNull(marketDescription1);
            Assert.IsNotNull(marketDescription2);
            Assert.IsNotNull(marketDescription3);
            Assert.AreEqual(marketId, marketDescription3.Id);

            _mockDataFetcher.Verify(x => x.GetDataAsync(It.IsAny<Uri>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void MarketDescriptionGetNonExistingTest()
        {
            var marketDescription = _marketDescriptionCache.GetMarketDescriptorAsync(56765, null, _cultures).Result;

            Assert.IsNotNull(marketDescription);
        }
    }
}