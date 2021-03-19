/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Test.Helpers
{
    public static class ConfigurationHelper
    {
        public const string ChannelParamName = "limitId";

        public static ISdkConfiguration BuilderMinimalConfiguration()
        {
            return MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .Build();
        }

        public static ISdkConfiguration BuilderSetters()
        {
            return MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetPort(1)
                .SetVirtualHost("/virtualHost")
                .SetUseSsl(false)
                .SetNode(2)
                .SetBookmakerId(3)
                .SetLimitId(4)
                .SetCurrency("EUR")
                .SetSenderChannel(SenderChannel.Internet)
                .SetAccessToken("accessToken")
                .SetProvideAdditionalMarketSpecifiers(false)
                .SetExclusiveConsumer(false)
                .SetKeycloakHost("keycloakHost")
                .SetKeycloakUsername("keycloakUsername")
                .SetKeycloakPassword("keycloakPassword")
                .SetKeycloakSecret("keycloakSecret")
                .SetMtsClientApiHost("clientApiHost")
                .SetTicketResponseTimeout(10000)
                .SetTicketCancellationResponseTimeout(10001)
                .SetTicketCashoutResponseTimeout(10002)
                .SetNonSrSettleResponseTimeout(10003)
                .Build();
        }
    }
}
