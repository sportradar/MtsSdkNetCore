using System;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.API.Internal.MtsAuth;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.REST;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;

namespace Sportradar.MTS.SDK.Test.Helpers
{
    internal class MtsAuthServiceHelper : IMtsAuthService
    {
        private readonly IMtsAuthService _mtsAuthService;

        public MtsAuthServiceHelper(ISdkConfiguration config = null)
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

            var uri = new Uri(newConfig.KeycloakHost + "/auth/realms/mts/protocol/openid-connect/token");

            var mockDataFetcher = new Mock<DataFetcherHelper>();
            mockDataFetcher.Setup(p => p.GetDataAsync(It.IsAny<Uri>())).Returns(new DataFetcherHelper(BuilderFactoryHelper.UriReplacements).GetDataAsync(uri));
            mockDataFetcher.Setup(p => p.PostDataAsync(It.IsAny<Uri>(), It.IsAny<HttpContent>())).Returns(new DataFetcherHelper(BuilderFactoryHelper.UriReplacements).PostDataAsync(uri, null));

            var deserializer = new SDK.Entities.Internal.JsonDeserializer<AccessTokenDTO>();
            var mapper = new KeycloakAuthorizationMapperFactory();

            var dataProvider = new DataProvider<AccessTokenDTO, KeycloakAuthorization>(
                uri.AbsoluteUri,
                mockDataFetcher.Object,
                mockDataFetcher.Object,
                deserializer,
                mapper);

            _mtsAuthService = new MtsAuthService(dataProvider, newConfig, null);
        }

        /// <summary>
        /// Gets the access token asynchronous.
        /// </summary>
        /// <param name="keycloackUsername">The keycloack username.</param>
        /// <param name="keycloackPassword">The keycloack password.</param>
        /// <returns>Access token if access granted</returns>
        public async Task<string> GetTokenAsync(string keycloackUsername = null, string keycloackPassword = null)
        {
            return await _mtsAuthService.GetTokenAsync(keycloackUsername, keycloackPassword).ConfigureAwait(false);
        }
    }
}
