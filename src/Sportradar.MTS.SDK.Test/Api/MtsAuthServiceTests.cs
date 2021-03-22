using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.API.Internal;
using Sportradar.MTS.SDK.API.Internal.MtsAuth;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Cache;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;
using Sportradar.MTS.SDK.Entities.Internal.REST.ReportApiImpl;
using Sportradar.MTS.SDK.Test.Helpers;
using TinyCsvParser;

namespace Sportradar.MTS.SDK.Test.Api
{
    [TestClass]
    [DeploymentItem("TESTDATA/auth-token.json", "TESTDATA")]
    public class MtsAuthServiceTests
    {
        private const string FileName = "TESTDATA\\auth-token.json";
        private IDeserializer<AccessTokenDTO> _deserializer;
        private Mock<DataFetcherHelper> _mockDataFetcher;
        private KeycloakAuthorizationMapperFactory _mapper;
        private IDataProvider<KeycloakAuthorization> _dataProvider;
        private IMtsAuthService _mtsAuthService;

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

            var uri = new Uri(config.KeycloakHost + "/auth/realms/mts/protocol/openid-connect/token");

            _mockDataFetcher = new Mock<DataFetcherHelper>();
            _mockDataFetcher.Setup(p => p.GetDataAsync(It.IsAny<Uri>())).Returns(new DataFetcherHelper(BuilderFactoryHelper.UriReplacements).GetDataAsync(uri));
            _mockDataFetcher.Setup(p => p.PostDataAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>())).Returns(new DataFetcherHelper(BuilderFactoryHelper.UriReplacements).PostDataAsync(uri, null));

            _deserializer = new SDK.Entities.Internal.JsonDeserializer<AccessTokenDTO>();
            _mapper = new KeycloakAuthorizationMapperFactory();

            _dataProvider = new DataProvider<AccessTokenDTO, KeycloakAuthorization>(
                    uri.AbsoluteUri,
                    _mockDataFetcher.Object,
                    _mockDataFetcher.Object,
                    _deserializer,
                    _mapper);

            _mtsAuthService = new MtsAuthService(_dataProvider, config, null);
        }

        [TestMethod]
        public void InitOkTest()
        {
            Assert.IsNotNull(_mtsAuthService);
            var token = _mtsAuthService.GetTokenAsync().Result;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }

        [TestMethod]
        public void DeserializeTokenMessageTest()
        {
            var tokenPath = _mockDataFetcher.Object.GetPathWithReplacements(FileName);
            Assert.IsTrue(!string.IsNullOrEmpty(tokenPath));
            using StreamReader stream = File.OpenText(tokenPath);
            var tokenData = stream.ReadToEnd();
            Assert.IsTrue(!string.IsNullOrEmpty(tokenData));
            var tokenDto = new SDK.Entities.Internal.JsonDeserializer<AccessTokenDTO>().Deserialize<AccessTokenDTO>(tokenData);
            Assert.IsNotNull(tokenDto);
            Assert.IsTrue(!string.IsNullOrEmpty(tokenDto.Access_token));
        }

        [TestMethod]
        public void CheckArgumentsTest()
        {
            Assert.IsNotNull(_mtsAuthService);
            var token = _mtsAuthService.GetTokenAsync("keyUsername", "keyPassword").Result;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }

        [TestMethod]
        public void CheckMtsAuthServiceHelperTest()
        {
            var mtsAuthServiceHelper = new MtsAuthServiceHelper();
            Assert.IsNotNull(mtsAuthServiceHelper);
            var token = mtsAuthServiceHelper.GetTokenAsync("keyUsername", "keyPassword").Result;
            Assert.IsTrue(!string.IsNullOrEmpty(token));
        }
    }
}
