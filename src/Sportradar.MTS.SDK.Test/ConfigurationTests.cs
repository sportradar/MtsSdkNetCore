/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Test.Helpers;

namespace Sportradar.MTS.SDK.Test
{
    [TestClass]
    public class ConfigurationTests
    {
        #region Minimal configuration

        [TestMethod]
        public void BuilderMinimalConfiguration()
        {
            var config = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .Build();

            CheckAllSettings(config);
        }

        [TestMethod]
        public void ConfigMinimalConfiguration()
        {
            var config = @"<mtsSdkSection username=""username"" password=""password"" host=""host"" />"
                .ToSdkConfiguration();

            CheckAllSettings(config);
        }

        #endregion

        #region Setters with valid settings

        [TestMethod]
        public void BuilderSetters()
        {
            var config = MtsSdk.CreateConfigurationBuilder()
                               .SetUsername("username")
                               .SetPassword("password")
                               .SetHost("host")
                               .SetPort(1)
                               .SetVirtualHost("/virtualHost")
                               .SetUseSsl(false)
                               .SetSslServerName("sslServerName")
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

            CheckAllSettings(
                             config,
                             "username",
                             "password",
                             "host",
                             1,
                             "/virtualHost",
                             false,
                             "sslServerName",
                             2,
                             3,
                             4,
                             "EUR",
                             SenderChannel.Internet,
                             "accessToken",
                             false,
                             false,
                             3600,
                             1000000,
                             false,
                             "keycloakHost",
                             "keycloakUsername",
                             "keycloakPassword",
                             "keycloakSecret",
                             "clientApiHost",
                             10000,
                             10000,
                             10001,
                             10002,
                             10003);
        }

        [TestMethod]
        public void BuilderSettersLivePrematch()
        {
            var config = MtsSdk.CreateConfigurationBuilder()
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
                               .SetTicketResponseTimeoutLive(10010)
                               .SetTicketResponseTimeoutPrematch(10005)
                               .SetTicketCancellationResponseTimeout(10001)
                               .SetTicketCashoutResponseTimeout(10002)
                               .SetNonSrSettleResponseTimeout(10003)
                               .Build();

            CheckAllSettings(
                             config,
                             "username",
                             "password",
                             "host",
                             1,
                             "/virtualHost",
                             false,
                             null,
                             2,
                             3,
                             4,
                             "EUR",
                             SenderChannel.Internet,
                             "accessToken",
                             false,
                             false,
                             3600,
                             1000000,
                             false,
                             "keycloakHost",
                             "keycloakUsername",
                             "keycloakPassword",
                             "keycloakSecret",
                             "clientApiHost",
                             10010,
                             10005,
                             10001,
                             10002,
                             10003);
        }

        [TestMethod]
        public void ConfigSetters()
        {
            var config = @"<mtsSdkSection
                username=""username""
                password=""password""
                host=""host""
                port=""1""
                vhost=""/virtualHost""
                useSsl=""false""
                sslServerName=""sslServerName""
                node=""2""
                bookmakerId=""3""
                limitId=""4""
                currency=""EUR""
                channel=""Internet""
                accessToken=""accessToken""
                provideAdditionalMarketSpecifiers=""false""
                statsEnabled=""true""
                statsTimeout=""36""
                statsMaxRecord=""10""
                exclusiveConsumer=""false""
                keycloakHost=""keycloakHost""
                keycloakUsername=""keycloakUsername""
                keycloakPassword=""keycloakPassword""
                keycloakSecret=""keycloakSecret""
                mtsClientApiHost=""clientApiHost""
                ticketResponseTimeout=""10000""
                ticketCancellationResponseTimeout=""10001""
                ticketCashoutResponseTimeout=""10002""
                ticketNonSrSettleResponseTimeout=""10003""/>"
                .ToSdkConfiguration();

            CheckAllSettings(
                             config,
                             "username",
                             "password",
                             "host",
                             1,
                             "/virtualHost",
                             false,
                             "sslServerName",
                             2,
                             3,
                             4,
                             "EUR",
                             SenderChannel.Internet,
                             "accessToken",
                             false,
                             true,
                             36,
                             10,
                             false,
                             "keycloakHost",
                             "keycloakUsername",
                             "keycloakPassword",
                             "keycloakSecret",
                             "clientApiHost",
                             10000,
                             SdkInfo.TicketResponseTimeoutPrematchDefault,
                             10001,
                             10002,
                             10003);
        }

        [TestMethod]
        public void ConfigSettersPrematch()
        {
            var config = @"<mtsSdkSection
                username=""username""
                password=""password""
                host=""host""
                port=""1""
                vhost=""/virtualHost""
                useSsl=""false""
                node=""2""
                bookmakerId=""3""
                limitId=""4""
                currency=""EUR""
                channel=""Internet""
                accessToken=""accessToken""
                provideAdditionalMarketSpecifiers=""false""
                statsEnabled=""true""
                statsTimeout=""36""
                statsMaxRecord=""10""
                exclusiveConsumer=""false""
                keycloakHost=""keycloakHost""
                keycloakUsername=""keycloakUsername""
                keycloakPassword=""keycloakPassword""
                keycloakSecret=""keycloakSecret""
                mtsClientApiHost=""clientApiHost""
                ticketResponseTimeout=""10010""
                ticketResponseTimeoutPrematch=""10005""
                ticketCancellationResponseTimeout=""10001""
                ticketCashoutResponseTimeout=""10002""
                ticketNonSrSettleResponseTimeout=""10003""/>"
                .ToSdkConfiguration();

            CheckAllSettings(
                             config,
                             "username",
                             "password",
                             "host",
                             1,
                             "/virtualHost",
                             false,
                             null,
                             2,
                             3,
                             4,
                             "EUR",
                             SenderChannel.Internet,
                             "accessToken",
                             false,
                             true,
                             36,
                             10,
                             false,
                             "keycloakHost",
                             "keycloakUsername",
                             "keycloakPassword",
                             "keycloakSecret",
                             "clientApiHost",
                             10010,
                             10005,
                             10001,
                             10002,
                             10003);
        }

        #endregion

        #region Missing required settings

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuilderMissingUsername()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetPassword("password")
                .SetHost("host")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void ConfigMissingUsername()
        {
            @"<mtsSdkSection password=""password"" host=""host"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuilderMissingPassword()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetHost("host")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void ConfigMissingPassword()
        {
            @"<mtsSdkSection username=""username"" host=""host"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuilderMissingHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(TargetInvocationException))]
        public void ConfigMissingHost()
        {
            @"<mtsSdkSection username=""username"" password=""password"" />"
                .ToSdkConfiguration();
        }

        #endregion

        #region Setters with invalid required settings

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyUsername()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("")
                .SetPassword("password")
                .SetHost("host")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullUsername()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername(null)
                .SetPassword("password")
                .SetHost("host")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigEmptyUsername()
        {
            @"<mtsSdkSection username="""" password=""password"" host=""host"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyPassword()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("")
                .SetHost("host")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullPassword()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword(null)
                .SetHost("host")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigEmptyPassword()
        {
            @"<mtsSdkSection username=""username"" password="""" host=""host"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigEmptyHost()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host="""" />"
                .ToSdkConfiguration();
        }

        #endregion

        #region Setters with invalid optional settings

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNegativePort()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetPort(-1)
                .Build();
        }

        [TestMethod]
        public void ConfigNegativePort()
        {
            var config = @"<mtsSdkSection username=""username"" password=""password"" host=""host"" port=""-1"" />"
                .ToSdkConfiguration();

            Assert.AreEqual(5671, config.Port);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyVirtualHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetVirtualHost("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullVirtualHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetVirtualHost(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNegativeNodeId()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetNode(-1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigNegativeNodeId()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host=""host"" node=""-1"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNegativeBookmakerId()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetBookmakerId(-1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigNegativeBookmakerId()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host=""host"" bookmakerId=""-1"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNegativeLimitId()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetLimitId(-1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigNegativeLimitId()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host=""host"" limitId=""-1"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderCurrencyToShort()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetCurrency("E")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigCurrencyToShort()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host=""host"" currency=""E"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderCurrencyToLong()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetCurrency("EUREUR")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigCurrencyToLong()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host=""host"" currency=""EUREUR"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullCurrency()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetCurrency(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyAccessToken()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetAccessToken("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullAccessToken()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetAccessToken(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyKeycloakHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakHost("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullKeycloakHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakHost(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyKeycloakUsername()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakUsername("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullKeycloakUsername()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakUsername(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyKeycloakPassword()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakPassword("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullKeycloakPassword()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakPassword(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyKeycloakSecret()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakSecret("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullKeycloakSecret()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetKeycloakSecret(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderEmptyClientApiHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetMtsClientApiHost("")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderNullClientApiHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetMtsClientApiHost(null)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderTicketResponseTimeoutToLow()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetTicketResponseTimeout(SdkInfo.TicketResponseTimeoutLiveMin - 1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigTicketResponseTimeoutToLow()
        {
            $@"<mtsSdkSection username=""username"" password=""password"" host=""host"" ticketResponseTimeout=""{SdkInfo.TicketResponseTimeoutLiveMin - 1}"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderTicketResponseTimeoutToHigh()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetTicketResponseTimeout(SdkInfo.TicketResponseTimeoutLiveMax + 1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigTicketResponseTimeoutToHigh()
        {
            $@"<mtsSdkSection username=""username"" password=""password"" host=""host"" ticketResponseTimeout=""{SdkInfo.TicketResponseTimeoutLiveMax + 1}"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderTicketCancellationResponseTimeoutToLow()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetTicketCancellationResponseTimeout(SdkInfo.TicketCancellationResponseTimeoutMin - 1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigTicketCancellationResponseTimeoutToLow()
        {
            $@"<mtsSdkSection username=""username"" password=""password"" host=""host"" ticketCancellationResponseTimeout=""{SdkInfo.TicketCancellationResponseTimeoutMin - 1}"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderTicketCancellationResponseTimeoutToHigh()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetTicketCancellationResponseTimeout(SdkInfo.TicketCancellationResponseTimeoutMax + 1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigTicketCancellationResponseTimeoutToHigh()
        {
            $@"<mtsSdkSection username=""username"" password=""password"" host=""host"" ticketCancellationResponseTimeout=""{SdkInfo.TicketCancellationResponseTimeoutMax + 1}"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderTicketCashoutResponseTimeoutToLow()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetTicketCashoutResponseTimeout(SdkInfo.TicketCashoutResponseTimeoutMin - 1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigTicketCashoutResponseTimeoutToLow()
        {
            $@"<mtsSdkSection username=""username"" password=""password"" host=""host"" ticketCashoutResponseTimeout=""{SdkInfo.TicketCashoutResponseTimeoutMin - 1}"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuilderTicketCashoutResponseTimeoutToHigh()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetTicketCashoutResponseTimeout(SdkInfo.TicketCashoutResponseTimeoutMax + 1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ConfigTicketCashoutResponseTimeoutToHigh()
        {
            $@"<mtsSdkSection username=""username"" password=""password"" host=""host"" ticketCashoutResponseTimeout=""{SdkInfo.TicketCashoutResponseTimeoutMax + 1}"" />"
                .ToSdkConfiguration();
        }

        #endregion

        #region Dependant settings

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuilderMissingKeycloakHost()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetMtsClientApiHost("clientApi")
                .SetKeycloakSecret("secret")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigMissingKeycloakHost()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host=""host"" mtsClientApiHost=""clientApi"" keycloakSecret=""secret"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuilderMissingKeycloakSecret()
        {
            MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetMtsClientApiHost("clientApi")
                .SetKeycloakHost("keycloak")
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigMissingKeycloakSecret()
        {
            @"<mtsSdkSection username=""username"" password=""password"" host=""host"" mtsClientApiHost=""clientApi"" keycloakHost=""keycloak"" />"
                .ToSdkConfiguration();
        }

        [TestMethod]
        public void BuilderMissingKeycloakUsernameAndPassword()
        {
            var sdkConfiguration = MtsSdk.CreateConfigurationBuilder()
                .SetUsername("username")
                .SetPassword("password")
                .SetHost("host")
                .SetMtsClientApiHost("clientApi")
                .SetKeycloakHost("keycloak")
                .SetKeycloakSecret("secret")
                .Build();
            Assert.IsNotNull(sdkConfiguration);
        }

        [TestMethod]
        public void ConfigMissingKeycloakUsernameAndPassword()
        {
            var sdkConfiguration = @"<mtsSdkSection username=""username"" password=""password"" host=""host"" mtsClientApiHost=""clientApi"" keycloakHost=""keycloak"" keycloakSecret=""secret"" />"
                .ToSdkConfiguration();
            Assert.IsNotNull(sdkConfiguration);
        }

        #endregion

        private static void CheckAllSettings(
            ISdkConfiguration config,
            string username = "username",
            string password = "password",
            string host = "host",
            int port = 5671,
            string virtualHost = "/username",
            bool useSsl = true,
            string sslServerName = null,
            int nodeId = 1,
            int bookmakerId = 0,
            int limitId = 0,
            string currency = null,
            SenderChannel? channel = null,
            string accessToken = "",
            bool provideAdditionalMarketSpecifiers = true,
            bool statisticsEnabled = false,
            int statisticsTimeout = 3600,
            int statisticsRecordLimit = 1000000,
            bool exclusiveConsumer = true,
            string keycloakHost = null,
            string keycloakUsername = null,
            string keycloakPassword = null,
            string keycloakSecret = null,
            string mtsClientApiHost = null,
            int ticketResponseTimeoutLive = SdkInfo.TicketResponseTimeoutLiveDefault,
            int ticketResponseTimeoutPrematch = SdkInfo.TicketResponseTimeoutPrematchDefault,
            int ticketCancellationResponseTimeout = SdkInfo.TicketCancellationResponseTimeoutDefault,
            int ticketCashoutResponseTimeout = SdkInfo.TicketCashoutResponseTimeoutDefault,
            int ticketNonSrResponseTimeout = SdkInfo.TicketNonSrResponseTimeoutDefault)
        {
            Assert.AreEqual(username, config.Username);
            Assert.AreEqual(password, config.Password);
            Assert.AreEqual(host, config.Host);
            Assert.AreEqual(port, config.Port);
            Assert.AreEqual(virtualHost, config.VirtualHost);
            Assert.AreEqual(useSsl, config.UseSsl);
            Assert.AreEqual(sslServerName, config.SslServerName);
            Assert.AreEqual(nodeId, config.NodeId);
            Assert.AreEqual(bookmakerId, config.BookmakerId);
            Assert.AreEqual(limitId, config.LimitId);
            Assert.AreEqual(currency, config.Currency);
            Assert.AreEqual(channel, config.Channel);
            Assert.AreEqual(accessToken, config.AccessToken);
            Assert.AreEqual(provideAdditionalMarketSpecifiers, config.ProvideAdditionalMarketSpecifiers);
            Assert.AreEqual(statisticsEnabled, config.StatisticsEnabled);
            Assert.AreEqual(statisticsTimeout, config.StatisticsTimeout);
            Assert.AreEqual(statisticsRecordLimit, config.StatisticsRecordLimit);
            Assert.AreEqual(exclusiveConsumer, config.ExclusiveConsumer);
            Assert.AreEqual(keycloakHost, config.KeycloakHost);
            Assert.AreEqual(keycloakUsername, config.KeycloakUsername);
            Assert.AreEqual(keycloakPassword, config.KeycloakPassword);
            Assert.AreEqual(keycloakSecret, config.KeycloakSecret);
            Assert.AreEqual(mtsClientApiHost, config.MtsClientApiHost);
            Assert.AreEqual(ticketResponseTimeoutLive, config.TicketResponseTimeoutLive);
            Assert.AreEqual(ticketResponseTimeoutPrematch, config.TicketResponseTimeoutPrematch);
            Assert.AreEqual(ticketCancellationResponseTimeout, config.TicketCancellationResponseTimeout);
            Assert.AreEqual(ticketCashoutResponseTimeout, config.TicketCashoutResponseTimeout);
            Assert.AreEqual(ticketNonSrResponseTimeout, config.TicketNonSrSettleResponseTimeout);
        }
    }
}