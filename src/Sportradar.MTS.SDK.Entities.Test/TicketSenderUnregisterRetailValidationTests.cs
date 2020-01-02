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
    public class TicketSenderUnregisterRetailValidationTests
    {
        [TestMethod]
        public void limit_is_required()
        {
            var builder = new SenderBuilder(TestHelper.BuilderMinimalConfiguration())
                .SetSenderChannel(SenderChannel.Retail)
                .SetBookmakerId(23)
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
    }
}