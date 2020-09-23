/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Test.Helpers;

namespace Sportradar.MTS.SDK.Test.Builders
{
    [TestClass]
    public class SenderBuilderTests
    {
        private static IBuilderFactory _builderFactory;
        private static IEndCustomer _endCustomer;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            var configuration = new SdkConfigurationInternal(new SdkConfiguration("user", "pass", "host", "vhost", false, "sslServerName", 5, 12, 17, "GBP", SenderChannel.Mobile), null);
            _builderFactory = new BuilderFactoryHelper(configuration).BuilderFactory;
            _endCustomer = _builderFactory.CreateEndCustomerBuilder().SetId("customerId").SetIp(IPAddress.Loopback).SetDeviceId("deviceId").SetLanguageId("si").Build();
        }

        [TestMethod]
        public void values_provided_to_setters_are_passed_to_instance()
        {
            var sender = _builderFactory.CreateSenderBuilder()
                .SetSenderChannel(SenderChannel.Mobile)
                .SetBookmakerId(12)
                .SetCurrency("gbp")
                .SetEndCustomer(_endCustomer)
                .SetLimitId(17)
                .Build();

            Assert.AreEqual(SenderChannel.Mobile, sender.Channel);
            Assert.AreEqual(12, sender.BookmakerId);
            Assert.AreEqual("GBP", sender.Currency);
            Assert.AreEqual(_endCustomer, sender.EndCustomer);
            Assert.AreEqual(17, sender.LimitId);
        }

        [TestMethod]
        public void config_value_are_passed_to_instance()
        {
            var sender = _builderFactory.CreateSenderBuilder()
                .SetEndCustomer(_endCustomer)
                .Build();

            Assert.AreEqual(SenderChannel.Mobile, sender.Channel);
            Assert.AreEqual(12, sender.BookmakerId);
            Assert.AreEqual("GBP", sender.Currency);
            Assert.AreEqual(_endCustomer, sender.EndCustomer);
            Assert.AreEqual(17, sender.LimitId);
        }

        [TestMethod]
        public void config_value_are_overridden_by_setter_value()
        {
            var sender = _builderFactory.CreateSenderBuilder()
                .SetSenderChannel(SenderChannel.Internet)
                .SetBookmakerId(13)
                .SetCurrency("EUR")
                .SetEndCustomer(_endCustomer)
                .SetLimitId(18)
                .Build();

            Assert.AreEqual(SenderChannel.Internet, sender.Channel);
            Assert.AreEqual(13, sender.BookmakerId);
            Assert.AreEqual("EUR", sender.Currency);
            Assert.AreEqual(_endCustomer, sender.EndCustomer);
            Assert.AreEqual(18, sender.LimitId);
        }

        [TestMethod]
        public void ValidCurrencyTest()
        {
            var sender = _builderFactory.CreateSenderBuilder()
                .SetCurrency("eur")
                .SetSenderChannel(SenderChannel.Internet)
                .SetBookmakerId(1)
                .SetEndCustomer(_endCustomer)
                .SetLimitId(1)
                .Build();
            Assert.AreEqual("EUR", sender.Currency);

            sender = _builderFactory.CreateSenderBuilder()
                .SetCurrency("mBTC")
                .SetSenderChannel(SenderChannel.Internet)
                .SetBookmakerId(1)
                .SetEndCustomer(_endCustomer)
                .SetLimitId(1)
                .Build();
            Assert.AreEqual("mBTC", sender.Currency);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TooShortCurrencyTest()
        {
            _builderFactory.CreateSenderBuilder()
                .SetCurrency("eu")
                .SetSenderChannel(SenderChannel.Internet)
                .SetBookmakerId(1)
                .SetEndCustomer(_endCustomer)
                .SetLimitId(1)
                .Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TooLongCurrencyTest()
        {
            _builderFactory.CreateSenderBuilder()
                .SetCurrency("mmBTC")
                .SetSenderChannel(SenderChannel.Internet)
                .SetBookmakerId(1)
                .SetEndCustomer(_endCustomer)
                .SetLimitId(1)
                .Build();
        }
    }
}