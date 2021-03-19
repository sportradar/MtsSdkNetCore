/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal.Builders;
using Sportradar.MTS.SDK.Test.Helpers;

namespace Sportradar.MTS.SDK.Test.Entities
{
    [TestClass]
    public class TicketSenderUnregisterRetailValidationTests
    {
        [TestMethod]
        public void limit_is_required()
        {
            var builder = new SenderBuilder(ConfigurationHelper.BuilderMinimalConfiguration())
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
                Assert.AreEqual(ConfigurationHelper.ChannelParamName, ex.ParamName, "Argument exception for wrong argument was thrown");
            }
        }
    }
}