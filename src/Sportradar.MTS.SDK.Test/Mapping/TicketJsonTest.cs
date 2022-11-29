/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashoutResponse;
using Sportradar.MTS.SDK.Test.Helpers;
using SenderChannel = Sportradar.MTS.SDK.Entities.Enums.SenderChannel;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Sportradar.MTS.SDK.Test.Mapping
{
    [TestClass]
    public class TicketJsonTest
    {
        private IBuilderFactory _builderFactory;
        private ISenderBuilder _senderBuilder;
        private ISender _sender;

        [TestInitialize]
        public void Init()
        {
            _builderFactory = new BuilderFactoryHelper().BuilderFactory;
            _senderBuilder = _builderFactory.CreateSenderBuilder().SetBookmakerId(9985).SetLimitId(90).SetCurrency("EUR").SetSenderChannel(SenderChannel.Internet);
            _sender = _senderBuilder.SetEndCustomer(_builderFactory.CreateEndCustomerBuilder()
                                                                   .SetId("customer-client-" + SR.I1000)
                                                                   .SetConfidence(12039203)
                                                                   .SetIp(IPAddress.Loopback)
                                                                   .SetLanguageId("en")
                                                                   .SetDeviceId("123")
                                                                   .Build())
                                    .Build();
        }

        private void CheckRequiredFields(string json)
        {
            Assert.IsTrue(!string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains(TicketHelper.MtsTicketVersion), "Wrong ticket version");
            Assert.IsTrue(json.Contains("timestampUtc"), "missing timestamp");
            Assert.IsTrue(json.Contains("bets"), "missing bets");
            Assert.IsTrue(json.Contains("stake"), "missing stake");
            Assert.IsTrue(json.Contains("selections"), "missing selections");
            Assert.IsTrue(json.Contains("sender"), "missing sender");
            Assert.IsTrue(json.Contains("limitId"), "missing  limitId");
            Assert.IsTrue(json.Contains("bookmakerId"), "missing bookmakerId");
            Assert.IsTrue(json.Contains("currency"), "missing currency");
            Assert.IsTrue(json.Contains("channel"), "missing channel");
            Assert.IsTrue(json.Contains("ticketId"), "missing ticketId");
            Assert.IsTrue(json.Contains("eventId"), "missing eventId");
            Assert.IsTrue(json.Contains("odds"), "missing odds");
            Assert.IsTrue(json.Contains("testSource"), "missing testSource");
            Assert.IsTrue(json.Contains("selectedSystems"), "missing selectedSystems");
        }

        private void CheckMandatoryFields(string json)
        {
            Assert.IsTrue(!string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("version"), "missing version");
            Assert.IsTrue(json.Contains("timestampUtc"), "missing timestamp");
            Assert.IsTrue(json.Contains("ticketId"), "missing ticketId");
        }

        private void CheckResponseFields(string json)
        {
            Assert.IsTrue(!string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("version"), "missing version");
            Assert.IsTrue(json.Contains("signature"), "missing signature");
            Assert.IsTrue(json.Contains("ticketId"), "missing ticketId");
            Assert.IsTrue(json.Contains("status"), "missing status");
        }

        [TestMethod]
        public void CheckRequiredTicketFieldsTest()
        {
            var ticket = TicketBuilderHelper.GetTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);
            Assert.IsTrue(json.Contains("totalCombinations"), "missing totalCombinations");
        }

        [TestMethod]
        public void CheckTicketEnumValuesTest()
        {
            var ticket = TicketBuilderHelper.GetTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("totalCombinations"), "missing totalCombinations");
            Assert.IsTrue(json.Contains("total"), "missing total");
            Assert.IsTrue(json.Contains("all"), "missing all");
            Assert.IsTrue(json.Contains("internet"), "missing internet");
        }

        [TestMethod]
        public void CheckNoOddsChangeEnumValuesTest()
        {
            var ticket = TicketBuilderHelper.GetTicketBuilder(_sender, null, 1, 2).BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("totalCombinations"), "missing totalCombinations");
            Assert.IsTrue(!json.Contains("oddsChange"), "extra oddsChange");
            Assert.IsTrue(!json.Contains("higher"), "extra higher");
            Assert.IsTrue(!json.Contains("none"), "extra none");
        }

        [TestMethod]
        public void CheckOddsChangeEnumValuesTest()
        {
            var ticket = TicketBuilderHelper.GetTicketBuilder(_sender, null, 1, 2).SetOddsChange(OddsChangeType.Higher).BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("oddsChange"));
            Assert.IsTrue(json.Contains("higher"));
        }

        [TestMethod]
        public void CheckNoBetBonusTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P, StakeType.Total).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(!json.Contains("bonus"));
            Assert.IsTrue(!json.Contains("mode"));
        }

        [TestMethod]
        public void CheckWithBetBonusTest()
        {
            var json = CreateBonusPromoBetTicket(SR.I1000P, SR.I1000P, null, null, null);
            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("bonus"));
            Assert.IsTrue(json.Contains("mode"));
            Assert.IsTrue(json.Contains("all"));
            Assert.IsTrue(json.Contains("total"));
            Assert.IsTrue(json.Contains("unit"));
        }

        [TestMethod]
        public void CheckNoStakeTypeTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("stake"), "missing stake");
            Assert.IsTrue(!json.Contains("type"), "extra type");
            Assert.IsTrue(!json.Contains("total"), "extra total");
            Assert.IsTrue(!json.Contains("unit"), "extra unit");
        }

        [TestMethod]
        public void CheckWithStakeTypeTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P, StakeType.Unit).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("stake"), "missing stake");
            Assert.IsTrue(json.Contains("type"), "missing type");
            Assert.IsTrue(!json.Contains("total"), "extra total");
            Assert.IsTrue(json.Contains("unit"), "missing unit");
        }

        [TestMethod]
        public void CheckNoBetIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P, StakeType.Total).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            Assert.IsNotNull(json);
            Assert.IsTrue(!string.IsNullOrEmpty(json));

            CheckRequiredFields(json);

            var i = json.IndexOf("bets", StringComparison.InvariantCultureIgnoreCase);
            var betSection = json.Substring(i > 0 ? i : 0);
            i = betSection.IndexOf("ticketId", StringComparison.InvariantCultureIgnoreCase);
            betSection = betSection.Substring(0, i > 0 ? i : betSection.Length);

            Assert.IsTrue(!betSection.Contains("id"));
        }

        [TestMethod]
        public void CheckWithBetIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P, StakeType.Total).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            Assert.IsNotNull(json);
            Assert.IsTrue(!string.IsNullOrEmpty(json));

            CheckRequiredFields(json);

            var i = json.IndexOf("bets", StringComparison.InvariantCultureIgnoreCase);
            var betSection = json.Substring(i > 0 ? i : 0);
            i = betSection.IndexOf("ticketId", StringComparison.InvariantCultureIgnoreCase);
            betSection = betSection.Substring(0, i > 0 ? i : betSection.Length);

            Assert.IsTrue(betSection.Contains("id"));
            Assert.IsTrue(betSection.Contains("bet-id-"));
        }

        [TestMethod]
        public void CheckNoTicketReofferRefIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender, null, 2, 2);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(!json.Contains("reofferRefId"));
        }

        [TestMethod]
        public void CheckWithTicketReofferRefIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender, null, 2, 2);

            var reofferRefId = "R-" + SR.S1000;
            ticketBuilder.SetReofferId(reofferRefId);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("reofferRefId"));
            Assert.IsTrue(json.Contains(reofferRefId));
        }

        [TestMethod]
        public void CheckNoTicketAltStakeRefIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender, null, 2, 2);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(!json.Contains("altStakeRefId"));
        }

        [TestMethod]
        public void CheckWithTicketAltStakeRefIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender, null, 2, 2);

            var altStakeRefId = "A-" + SR.S1000;
            ticketBuilder.SetAltStakeRefId(altStakeRefId);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("altStakeRefId"));
            Assert.IsTrue(json.Contains(altStakeRefId));
        }

        [TestMethod]
        public void CheckWithAutoTicketReofferRefIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender, null, 1, 1);

            var ticket = ticketBuilder.BuildTicket();
            var reofferRefId = "R-" + SR.S1000;
            var reofferTicket = _builderFactory.CreateTicketReofferBuilder().Set(ticket, 200000, reofferRefId).BuildTicket();
            var dto = new TicketMapper().Map(reofferTicket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("reofferRefId"));
            Assert.IsTrue(json.Contains(ticket.TicketId));
            Assert.IsTrue(json.Contains(reofferRefId));
            Assert.IsTrue(json.Contains("200000"));
        }

        [TestMethod]
        public void CheckWithAutoTicketAltStakeRefIdTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender, null, 1, 1);

            var ticket = ticketBuilder.BuildTicket();
            var altStakeRefId = "A-" + SR.S1000;
            var altStakeTicket = _builderFactory.CreateAltStakeBuilder().Set(ticket, 200000, altStakeRefId).BuildTicket();
            var dto = new TicketMapper().Map(altStakeTicket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("altStakeRefId"));
            Assert.IsTrue(json.Contains(ticket.TicketId));
            Assert.IsTrue(json.Contains(altStakeRefId));
            Assert.IsTrue(json.Contains("200000"));
        }

        [TestMethod]
        public void CheckNoSumOfWinsTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P, StakeType.Total).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(!json.Contains("sumOfWins"));
        }

        [TestMethod]
        public void CheckWithSumOfWinsTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P, StakeType.Total).SetSumOfWins(123456).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("sumOfWins"));
            Assert.IsTrue(json.Contains("123456"));
            Assert.IsTrue(!json.Contains("banker"));
        }

        [TestMethod]
        public void CheckWithSelectionBankerTest()
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(true).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(SR.I1000P, StakeType.Total).SetSumOfWins(123456).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("banker"));
            Assert.IsTrue(json.Contains("selectionRefs"));
        }

        [TestMethod]
        public void CheckNoEndCustomerConfidenceTest()
        {
            var endCustomer = _builderFactory.CreateEndCustomerBuilder()
                                    .SetId("customer-client-" + SR.I1000)
                                    .SetIp(IPAddress.Loopback)
                                    .SetLanguageId("en")
                                    .SetDeviceId("123")
                                    .Build();
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(null, endCustomer, 5, 10);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(!json.Contains("confidence"));
        }

        [TestMethod]
        public void CheckWithEndCustomerConfidenceTest()
        {
            var endCustomer = _builderFactory.CreateEndCustomerBuilder()
                                    .SetId("customer-client-" + SR.I1000)
                                    .SetConfidence(123456)
                                    .SetIp(IPAddress.Loopback)
                                    .SetLanguageId("en")
                                    .SetDeviceId("123")
                                    .Build();
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(null, endCustomer, 5, 10);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("confidence"));
            Assert.IsTrue(json.Contains("123456"));
        }

        [TestMethod]
        public void CheckWithEmptyEndCustomerTest()
        {
            var endCustomer = _builderFactory.CreateEndCustomerBuilder().Build();

            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(null, endCustomer, 1, 1);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("endCustomer"));
            Assert.IsTrue(!json.Contains("\"ip\":"));
            Assert.IsTrue(!json.Contains("deviceId"));
            Assert.IsTrue(!json.Contains("languageId"));
            Assert.IsTrue(!json.Contains("confidence"));
        }

        [TestMethod]
        public void CheckWithFullEndCustomerTest()
        {
            var endCustomer = _builderFactory.CreateEndCustomerBuilder()
                                    .SetId("customer-client-" + SR.I1000)
                                    .SetConfidence(123456)
                                    .SetIp(IPAddress.Loopback)
                                    .SetLanguageId("en")
                                    .SetDeviceId("123")
                                    .Build();
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(null, endCustomer, 1, 1);

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("endCustomer"));
            Assert.IsTrue(json.Contains("ip"));
            Assert.IsTrue(json.Contains("deviceId"));
            Assert.IsTrue(json.Contains("languageId"));
            Assert.IsTrue(json.Contains("confidence"));
        }

        [TestMethod]
        public void TicketToJsonTest()
        {
            var ticket = TicketBuilderHelper.GetTicket();
            var dto = new TicketMapper().Map(ticket);
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckRequiredFields(ticketJson);
            CheckRequiredFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketCancelToJsonTest()
        {
            var ticket = TicketBuilderHelper.GetTicketCancel();
            var dto = new TicketCancelMapper().Map(ticket);
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckMandatoryFields(ticketJson);
            CheckMandatoryFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketAckToJsonTest()
        {
            var ticket = TicketBuilderHelper.GetTicketAck();
            var dto = new TicketAckMapper().Map(ticket);
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckMandatoryFields(ticketJson);
            CheckMandatoryFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketCancelAckToJsonTest()
        {
            var ticket = TicketBuilderHelper.GetTicketCancelAck();
            var dto = new TicketCancelAckMapper().Map(ticket);
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckMandatoryFields(ticketJson);
            CheckMandatoryFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketCashoutToJsonTest()
        {
            var ticket = TicketBuilderHelper.GetTicketCashout();
            var dto = new TicketCashoutMapper().Map(ticket);
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckMandatoryFields(ticketJson);
            CheckMandatoryFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketReofferCancelToJsonTest()
        {
            var ticket = TicketBuilderHelper.GetTicketReofferCancel();
            var dto = new TicketReofferCancelMapper().Map(ticket);
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckMandatoryFields(ticketJson);
            CheckMandatoryFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketResponseToJsonTest()
        {
            var dto = TicketBuilderHelper.GetTicketResponse();
            var ticket = new TicketResponseMapper(null).Map(dto, "c1", null, dto.ToJson());
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckResponseFields(ticketJson);
            CheckResponseFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketCancelResponseToJsonTest()
        {
            var dto = TicketBuilderHelper.GetTicketCancelResponse();
            var ticket = new TicketCancelResponseMapper(null).Map(dto, "c1", null, dto.ToJson());
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckResponseFields(ticketJson);
            CheckResponseFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void TicketCashoutResponseToJsonTest()
        {
            var dto = TicketBuilderHelper.GetTicketCashoutResponse(SR.S1000, Status.Accepted);
            var ticket = new TicketCashoutResponseMapper().Map(dto, "c1", null, dto.ToJson());
            var ticketJson = ticket.ToJson();
            var dtoJson = dto.ToJson();

            CheckResponseFields(ticketJson);
            CheckResponseFields(dtoJson);

            Assert.AreEqual(ticketJson, dtoJson);
        }

        [TestMethod]
        public void CheckWithBetBonusAccumulatorCashTest()
        {
            var json = CreateBonusPromoBetTicket(SR.I1000P, SR.I1000P, BetBonusDescription.AccaBonus, BetBonusPaidAs.Cash, null);
            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("bonus"));
            Assert.IsTrue(json.Contains("\"mode\":\"all\""));
            Assert.IsTrue(json.Contains("\"type\":\"total\""));
            Assert.IsTrue(json.Contains("\"description\":\"accaBonus\""));
            Assert.IsTrue(json.Contains("\"paidAs\":\"cash\""));
        }

        [TestMethod]
        public void CheckWithBetBonusAccumulatorFreeBetTest()
        {
            var json = CreateBonusPromoBetTicket(SR.I1000P, SR.I1000P, BetBonusDescription.Other, BetBonusPaidAs.FreeBet, null);
            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("bonus"));
            Assert.IsTrue(json.Contains("\"mode\":\"all\""));
            Assert.IsTrue(json.Contains("\"type\":\"total\""));
            Assert.IsTrue(json.Contains("\"description\":\"other\""));
            Assert.IsTrue(json.Contains("\"paidAs\":\"freeBet\""));
        }

        [TestMethod]
        public void CheckWithBetBonusAccumulatorBoostedOddsTest()
        {
            var json = CreateBonusPromoBetTicket(SR.I1000P, SR.I1000P, BetBonusDescription.OddsBooster, BetBonusPaidAs.Cash, SR.I1000P + 1000);
            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("bonus"));
            Assert.IsTrue(json.Contains("\"mode\":\"all\""));
            Assert.IsTrue(json.Contains("\"type\":\"total\""));
            Assert.IsTrue(json.Contains("\"description\":\"oddsBooster\""));
            Assert.IsTrue(json.Contains("\"paidAs\":\"cash\""));
            Assert.IsTrue(json.Contains("\"boostedOdds\":"));
        }

        [TestMethod]
        public void CheckWithFreeStakeCashMoneyBackTotalTest()
        {
            var json = CreateFreeStakeBetTicket(SR.I1000P, FreeStakeType.Total, FreeStakeDescription.MoneyBack, FreeStakePaidAs.Cash, false);
            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("freeStake"));
            Assert.IsTrue(json.Contains("\"type\":\"total\""));
            Assert.IsTrue(json.Contains("\"description\":\"moneyBack\""));
            Assert.IsTrue(json.Contains("\"paidAs\":\"cash\""));
        }

        [TestMethod]
        public void CheckWithFreeStakeFreeBetFreeBetUnitWithStakeZeroTest()
        {
            var json = CreateFreeStakeBetTicket(SR.I1000P, FreeStakeType.Unit, FreeStakeDescription.FreeBet, FreeStakePaidAs.FreeBet, true);
            CheckRequiredFields(json);

            Assert.IsTrue(json.Contains("freeStake"));
            Assert.IsTrue(TestHelper.ContainsCount(json, "\"type\":\"unit\"", StringComparison.OrdinalIgnoreCase) > 1);
            Assert.IsTrue(json.Contains("\"description\":\"freeBet\""));
            Assert.IsTrue(json.Contains("\"paidAs\":\"freeBet\""));
            Assert.IsTrue(json.Contains("\"version\":\"2.4\""));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void CheckWithBetBonusWithStakeZeroTest()
        {
            var json = CreateBonusPromoBetTicket(0, SR.I1000P, null, null, null);
            CheckRequiredFields(json);
        }

        private string CreateBonusPromoBetTicket(long value, long bonusValue, BetBonusDescription? description, BetBonusPaidAs? paidAs, int? boostedOdds)
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.I1000P.ToString()).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBoostedOdds(boostedOdds).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(value, StakeType.Unit).SetBetBonus(bonusValue, BetBonusMode.All, BetBonusType.Total, description, paidAs).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);
            ticketBuilder.SetTotalCombinations(ticketBuilder.GetBets().Count());

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            return dto.ToJson();
        }

        private string CreateFreeStakeBetTicket(long value, FreeStakeType? type, FreeStakeDescription? description, FreeStakePaidAs? paidAs, bool zeroStake)
        {
            var ticketBuilder = TicketBuilderHelper.GetTicketBuilder(_sender);

            var betBuilder = _builderFactory.CreateBetBuilder();
            betBuilder.AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId(SR.I1000P.ToString()).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());

            var bet = betBuilder.AddSelectedSystem(1).SetStake(zeroStake ? 0 : SR.I1000P, StakeType.Unit).SetFreeStake(value, type, description, paidAs).SetBetId("bet-id-" + SR.I1000).Build();
            ticketBuilder.AddBet(bet);
            ticketBuilder.SetTotalCombinations(ticketBuilder.GetBets().Count());

            var ticket = ticketBuilder.BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            return dto.ToJson();
        }
    }
}