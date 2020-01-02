/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal.Builders;

namespace Sportradar.MTS.SDK.Entities.Test
{
    [TestClass]
    public class TicketSenderUnregisteredTerminalValidationTests
    {
        [TestMethod]
        public void limit_is_required()
        {
            var builder = new SenderBuilder(TestHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.Terminal)
                .SetTerminalId("terminal")
                .SetBookmakerId(1)
                .SetCurrency("eur");

            try
            {
                builder.Build();
                Assert.Fail("Build should throw an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(TestHelper.ChannelParamName, ex.ParamName, "Argument exception for wrong argument was thrown");
            }
        }

        [TestMethod]
        public void shop_id_is_allowed()
        {
            var builder = new SenderBuilder(TestHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.Terminal)
                .SetLimitId(1)
                .SetTerminalId("terminal")
                .SetShopId("shop")
                .SetBookmakerId(1)
                .SetCurrency("eur");

            var sender = builder.Build();
            Assert.IsNotNull(builder);
            Assert.IsNotNull(sender);
        }

        [TestMethod]
        public void valid_sender_is_validated()
        {
            var builder = new SenderBuilder(TestHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.Terminal)
                .SetLimitId(1)
                .SetTerminalId("terminal")
                .SetBookmakerId(1)
                .SetCurrency("eur");

            var sender = builder.Build();
            Assert.IsNotNull(sender);
        }
    }
}