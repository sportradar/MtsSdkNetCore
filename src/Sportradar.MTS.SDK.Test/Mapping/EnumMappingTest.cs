/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;
using StakeType = Sportradar.MTS.SDK.Entities.Enums.StakeType;

namespace Sportradar.MTS.SDK.Test.Mapping
{
    [TestClass]
    public class EnumMappingTest
    {
        [TestMethod]
        public void BetBonusModeEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<BetBonusMode, BonusMode>());
        }
        [TestMethod]
        public void BetBonusTypeEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<BetBonusType, BonusType>());
        }
        [TestMethod]
        public void BetReofferTypeEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<BetReofferType, ReofferType>());
        }
        [TestMethod]
        public void OddsChangeTypeEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<OddsChangeType, TicketOddsChange>());
        }
        [TestMethod]
        public void StakeTypeEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<StakeType, SDK.Entities.Internal.Dto.Ticket.StakeType>());
        }
        [TestMethod]
        public void TicketAcceptanceEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<TicketAcceptance, Status>());
        }
        [TestMethod]
        public void TicketAckStatusEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<TicketAckStatus, TicketAckDTOTicketStatus>());
        }
        [TestMethod]
        public void TicketCancelAcceptanceEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<TicketCancelAcceptance, SDK.Entities.Internal.Dto.TicketCancelResponse.Status>());
        }
        [TestMethod]
        public void TicketCancelAckStatusEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<TicketCancelAckStatus, TicketCancelAckDTOTicketCancelStatus>());
        }
        [TestMethod]
        public void TicketCashoutResponseResultStatusEnumTest()
        {
            Assert.IsTrue(EnumsCanBeMapped<CashoutAcceptance, SDK.Entities.Internal.Dto.TicketCashoutResponse.Status>());
        }

        private static bool EnumsCanBeMapped<T1, T2>() where T1 : struct where T2 : struct
        {
            Assert.IsTrue(IsEnum<T1>());
            Assert.IsTrue(IsEnum<T2>());

            var names1 = Enum.GetNames(typeof(T1));
            var names2 = Enum.GetNames(typeof(T2));

            Assert.AreEqual(names1.Length, names2.Length);

            return true;
        }

        private static bool IsEnum<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T must be an enum");
            }
            return true;
        }
    }
}