/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal.Builders;
using Sportradar.MTS.SDK.Test.Helpers;

namespace Sportradar.MTS.SDK.Test.Entities
{
    [TestClass]
    public class TicketSenderCallCenterChannelValidationTests
    {
        [TestMethod]
        public void limit_is_required()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.CallCentre)
                .SetBookmakerId(1)
                .SetCurrency("eur")
                .SetEndCustomer(new EndCustomerBuilder().SetId("client").Build());

            try
            {
                builder.Build();
                Assert.Fail("Build should throw an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ConfigurationHelper.ChannelParamName, ex.ParamName, "Argument exception for wrong argument was thrown");
            }
        }

        [TestMethod]
        public void shop_id_is_allowed()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.CallCentre)
                .SetLimitId(1)
                .SetBookmakerId(1)
                .SetCurrency("eur")
                .SetShopId("shop")
                .SetEndCustomer(new EndCustomerBuilder().SetId("client").Build());
            var sender = builder.Build();
            Assert.IsNotNull(sender);
        }

        [TestMethod]
        public void terminal_id_is_allowed()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.CallCentre)
                .SetLimitId(1)
                .SetBookmakerId(1)
                .SetCurrency("eur")
                .SetTerminalId("terminal")
                .SetEndCustomer(new EndCustomerBuilder().SetId("client").Build());
            var sender = builder.Build();
            Assert.IsNotNull(sender);
        }

        [TestMethod]
        public void end_customer_ip_is_allowed()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.CallCentre)
                .SetLimitId(1)
                .SetBookmakerId(1)
                .SetCurrency("eur")
                .SetEndCustomer(new EndCustomerBuilder().SetId("client").SetIp(IPAddress.Loopback).Build());
            var sender = builder.Build();
            Assert.IsNotNull(sender);
        }

        [TestMethod]
        public void end_customer_language_is_allowed()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
                                       .SetSenderChannel(SenderChannel.CallCentre)
                                       .SetLimitId(1)
                                       .SetBookmakerId(1)
                                       .SetCurrency("eur")
                                       .SetEndCustomer(new EndCustomerBuilder().SetId("client").SetLanguageId("en").Build());

            var sender = builder.Build();
            Assert.IsNotNull(sender);

        }

        [TestMethod]
        public void end_customer_device_id_is_allowed()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
                                       .SetSenderChannel(SenderChannel.CallCentre)
                                       .SetLimitId(1)
                                       .SetBookmakerId(1)
                                       .SetCurrency("eur")
                                       .SetEndCustomer(new EndCustomerBuilder().SetId("client").SetDeviceId("device").Build());

            var sender = builder.Build();
            Assert.IsNotNull(sender);
        }

        [TestMethod]
        public void valid_sender_is_validated()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.CallCentre)
                .SetLimitId(1)
                .SetBookmakerId(1)
                .SetCurrency("eur")
                .SetEndCustomer(new EndCustomerBuilder().SetId("client").Build());

            var sender = builder.Build();
            Assert.IsNotNull(sender);
        }
    }
}