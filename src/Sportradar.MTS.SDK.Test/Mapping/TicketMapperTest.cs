/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.API.Internal.Mappers;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancel;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;
using Sportradar.MTS.SDK.Test.Helpers;
using SenderChannel = Sportradar.MTS.SDK.Entities.Enums.SenderChannel;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;
using StakeType = Sportradar.MTS.SDK.Entities.Enums.StakeType;
using Ticket = Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.Ticket;
// ReSharper disable RedundantArgumentDefaultValue

namespace Sportradar.MTS.SDK.Test.Mapping
{
    [TestClass]
    public class TicketMapperTest
    {
        private static readonly string DirPath = Directory.GetCurrentDirectory() + @"\JSON";
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

        [TestMethod]
        public void BuildTicketDtoFromTicketTest()
        {
            var ticket = TicketBuilderHelper.GetTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = new TicketDTO {Ticket = Ticket.FromJson(json)};
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildTicketCancelDtoFromTicketCancelTest()
        {
            var ticket = TicketBuilderHelper.GetTicketCancel();
            var dto = new TicketCancelMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = new TicketCancelDTO {Cancel = Cancel.FromJson(json)};
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildTicketAckDtoFromTicketAckTest()
        {
            var ticket = new TicketAck(SR.S1000, SR.I1000, TicketAckStatus.Rejected, SR.I100, SR.S1000);
            var dto = new TicketAckMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = TicketAckDTO.FromJson(json);
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildTicketCancelAckDtoFromTicketCancelAckTest()
        {
            var ticket = new TicketCancelAck(SR.S1000, SR.I1000, TicketCancelAckStatus.NotCancelled, SR.I100, SR.S1000);
            var dto = new TicketCancelAckMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = TicketCancelAckDTO.FromJson(json);
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildMultiTicketDtoFromTicketTest()
        {
            var ticket = TicketBuilderHelper.GetTicket(null, 0, 10, 3);
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = new TicketDTO {Ticket = Ticket.FromJson(json)};
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildTicketResponseDtoFromTicketResponseTest()
        {
            var dto = TicketBuilderHelper.GetTicketResponse();
            var ticket = new TicketResponseMapper(null).Map(dto, SR.S1000, null, dto.ToJson());
            Assert.IsNotNull(ticket);
            TicketCompareHelper.Compare(ticket, dto);
        }

        [TestMethod]
        public void BuildTicketCancelResponseDtoFromTicketCancelResponseTest()
        {
            var dto = TicketBuilderHelper.GetTicketCancelResponse();
            var ticket = new TicketCancelResponseMapper(null).Map(dto, SR.S1000, null, dto.ToJson());
            Assert.IsNotNull(ticket);
            TicketCompareHelper.Compare(ticket, dto);
        }

        [TestMethod]
        public void BuildTicketReofferCancelDtoFromTicketTestW()
        {
            var ticket = TicketBuilderHelper.GetTicketReofferCancel();
            var dto = new TicketReofferCancelMapper().Map(ticket);
            Assert.IsNotNull(dto);
            TicketCompareHelper.Compare(ticket, dto);
        }

        [TestMethod]
        public void BuildTicketCashoutDtoFromTicketTest()
        {
            var ticket = TicketBuilderHelper.GetTicketCashout();
            var dto = new TicketCashoutMapper().Map(ticket);
            Assert.IsNotNull(dto);
            TicketCompareHelper.Compare(ticket, dto);
        }

        [TestMethod]
        public void BuildTicketCashoutResponseDtoFromTicketCashoutResponseTest()
        {
            var dto = TicketBuilderHelper.GetTicketCashoutResponse(null, SDK.Entities.Internal.Dto.TicketCashoutResponse.Status.Rejected);
            var ticket = new TicketCashoutResponseMapper().Map(dto, SR.S1000, null, dto.ToJson());
            Assert.IsNotNull(ticket);
            TicketCompareHelper.Compare(ticket, dto);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-base.json", "JSON")]
        public void BuildTicketDtoFromJsonTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-base.json");
            var dto = Ticket.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Sender);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-multibet.json", "JSON")]
        public void BuildTicketDtoFromJsonMultiBetTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-multibet.json");
            var dto = Ticket.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Sender);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-multiselection.json", "JSON")]
        public void BuildTicketDtoFromJsonMultiSelectionTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-multiselection.json");
            var dto = Ticket.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Sender);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-cancel.json", "JSON")]
        public void BuildTicketCancelDtoFromJsonTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-cancel.json");
            var dto = Cancel.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.AreEqual(9985, dto.Sender.BookmakerId);
            Assert.AreEqual((int)TicketCancellationReason.BookmakerTechnicalIssue, dto.Code);
            Assert.AreEqual("2.0", dto.Version);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-ack.json", "JSON")]
        public void BuildTicketAckDtoFromJsonTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-ack.json");
            var dto = TicketAckDTO.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Sender);
            Assert.AreEqual(9985, dto.Sender.BookmakerId);
            Assert.AreEqual(TicketAckDTOTicketStatus.Accepted, dto.TicketStatus);
            Assert.AreEqual("2.0", dto.Version);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-cancel-ack.json", "JSON")]
        public void BuildTicketCancelAckDtoFromJsonTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-cancel-ack.json");
            var dto = TicketCancelAckDTO.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Sender);
            Assert.AreEqual(9985, dto.Sender.BookmakerId);
            Assert.AreEqual(TicketCancelAckDTOTicketCancelStatus.Cancelled, dto.TicketCancelStatus);
            Assert.AreEqual("2.0", dto.Version);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response.json", "JSON")]
        public void BuildTicketResponseDtoFromJsonTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response.json");
            var dto = TicketResponseDTO.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Result);
            Assert.AreEqual(Status.Accepted, dto.Result.Status);
            Assert.IsNotNull(dto.Result.Reason);
            Assert.AreEqual(1024, dto.Result.Reason.Code);
            Assert.AreEqual("2.0", dto.Version);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response2.json", "JSON")]
        public void BuildTicketResponseDtoFromJson2Test()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response2.json");
            var dto = TicketResponseDTO.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Result);
            Assert.AreEqual(Status.Rejected, dto.Result.Status);
            Assert.IsNotNull(dto.Result.Reason);
            Assert.AreEqual(-422, dto.Result.Reason.Code);
            Assert.AreEqual("2.0", dto.Version);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response2.json", "JSON")]
        public void BuildTicketResponseFromTicketResponseDtoTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response2.json");
            var dto = TicketResponseDTO.FromJson(json);
            var ticket = new TicketResponseMapper(null).Map(dto, SR.S1000, null, dto.ToJson());
            Assert.IsNotNull(ticket);
            TicketCompareHelper.Compare(ticket, dto);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response3.json", "JSON")]
        public void BuildTicketResponseDtoFromJson3_EmptySelectionDetailsTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response3.json");
            var dto = TicketResponseDTO.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Result);
            Assert.AreEqual(Status.Rejected, dto.Result.Status);
            Assert.IsNotNull(dto.Result.Reason);
            Assert.AreEqual(-321, dto.Result.Reason.Code);
            Assert.AreEqual("2.0", dto.Version);
            Assert.IsNotNull(dto.Result.BetDetails);
            Assert.IsNotNull(dto.Result.BetDetails.First().SelectionDetails);
            Assert.IsTrue(!dto.Result.BetDetails.First().SelectionDetails.Any());
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response3.json", "JSON")]
        public void BuildTicketResponseFromTicketResponseDto_EmptySelectionDetailsTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response3.json");
            var dto = TicketResponseDTO.FromJson(json);
            var ticket = new TicketResponseMapper(null).Map(dto, SR.S1000, null, dto.ToJson());
            TicketCompareHelper.Compare(ticket, dto);
            Assert.IsNotNull(ticket.BetDetails);
            Assert.IsNotNull(dto.Result.BetDetails.First().SelectionDetails);
            Assert.IsNull(ticket.BetDetails.First().SelectionDetails);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response4.json", "JSON")]
        public void BuildTicketResponseDtoFromJson4_MissingSelectionDetailsTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response4.json");
            var dto = TicketResponseDTO.FromJson(json);

            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Result);
            Assert.AreEqual(Status.Rejected, dto.Result.Status);
            Assert.IsNotNull(dto.Result.Reason);
            Assert.AreEqual(-321, dto.Result.Reason.Code);
            Assert.AreEqual("2.0", dto.Version);
            Assert.IsNotNull(dto.Result.BetDetails);
            Assert.IsNotNull(dto.Result.BetDetails.First().SelectionDetails);
            Assert.IsTrue(!dto.Result.BetDetails.First().SelectionDetails.Any());
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response4.json", "JSON")]
        public void BuildTicketResponseFromTicketResponseDto_MissingSelectionDetailsTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response4.json");
            var dto = TicketResponseDTO.FromJson(json);
            var ticket = new TicketResponseMapper(null).Map(dto, SR.S1000, null, dto.ToJson());
            TicketCompareHelper.Compare(ticket, dto);
            Assert.IsNotNull(ticket.BetDetails);
            Assert.IsNotNull(dto.Result.BetDetails.First().SelectionDetails);
            Assert.IsNull(ticket.BetDetails.First().SelectionDetails);
        }

        [TestMethod]
        public void ValidateDtoEnumValuesTest()
        {
            Assert.AreEqual(1, (int)TicketAckDTOTicketStatus.Accepted, "Wrong TicketAckDTOTicketStatus.Accepted");
            Assert.AreEqual(0, (int)TicketAckDTOTicketStatus.Rejected, "Wrong TicketAckDTOTicketStatus.Rejected");
            Assert.AreEqual(1, (int)TicketCancelAckDTOTicketCancelStatus.Cancelled, "Wrong TicketCancelAckDTOTicketCancelStatus.Cancelled");
            Assert.AreEqual(0, (int)TicketCancelAckDTOTicketCancelStatus.Not_cancelled, "Wrong TicketCancelAckDTOTicketCancelStatus.Not_Cancelled");
            Assert.AreEqual(1, (int)Status.Accepted, "Wrong TicketResponse.Status.Accepted");
            Assert.AreEqual(0, (int)Status.Rejected, "Wrong TicketResponse.Status.Rejected");
            Assert.AreEqual(1, (int)SDK.Entities.Internal.Dto.TicketCancelResponse.Status.Cancelled, "Wrong TicketCancelResponse.Status");
            Assert.AreEqual(0, (int)SDK.Entities.Internal.Dto.TicketCancelResponse.Status.Not_cancelled, "Wrong TicketCancelResponse.Status");
        }

        [TestMethod]
        public void ValidateBetIdPatternTest()
        {
            Assert.IsTrue(TicketHelper.ValidateTicketId("a"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("aAB"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("129837403"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("AaZYc"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("ticket:1"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("Test_12123"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("T:123_434"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("T::23123"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("::__r"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("-"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("Test-3423-324234-2343243"));
            Assert.IsTrue(TicketHelper.ValidateTicketId("B0034827552620261"));
        }

        [TestMethod]
        public void ValidateUserIdPatternTest()
        {
            Assert.IsTrue(TicketHelper.ValidateUserId("a"));
            Assert.IsTrue(TicketHelper.ValidateUserId("aAB"));
            Assert.IsTrue(TicketHelper.ValidateUserId("129837403"));
            Assert.IsTrue(TicketHelper.ValidateUserId("AaZYc"));
            Assert.IsTrue(TicketHelper.ValidateUserId("ticket:1"));
            Assert.IsTrue(TicketHelper.ValidateUserId("Test_12123"));
            Assert.IsTrue(TicketHelper.ValidateUserId("T:123_434"));
            Assert.IsTrue(TicketHelper.ValidateUserId("T::23123"));
            Assert.IsTrue(TicketHelper.ValidateUserId("::__r"));
            Assert.IsTrue(TicketHelper.ValidateUserId("-"));
            Assert.IsTrue(TicketHelper.ValidateUserId("Test-3423-324234-2343243"));
            Assert.IsTrue(TicketHelper.ValidateUserId("B0034827552620261"));
        }

        [TestMethod]
        public void BuildSelectionRefCorrectlyTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + new Random().Next(10000)).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetBetId("bet-id-" + SR.I1000).SetBetBonus(15000).SetStake(92343, StakeType.Total).AddSelectedSystem(1).AddSelectedSystem(2)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:2/sr:sport:1/500/1724?total=4.5").SetOdds(18000).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:3/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .Build())
                .AddBet(_builderFactory.CreateBetBuilder().SetBetId("bet-id-" + SR.I1000).SetBetBonus(10000).SetStake(92343, StakeType.Total).AddSelectedSystem(1)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:3/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);

            var dto = new TicketMapper().Map(ticket);
            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Ticket);

            Assert.AreEqual(3, dto.Ticket.Selections.Count());
            Assert.AreEqual(2, dto.Ticket.Bets.Count());

            var bet1 = dto.Ticket.Bets.First();
            Debug.Assert(bet1 != null, "bet1 != null");
            Assert.AreEqual(2, bet1.SelectedSystems.Count());
            Assert.IsNull(bet1.SelectionRefs);

            var bet2 = dto.Ticket.Bets.Last();
            Debug.Assert(bet2 != null, "bet2 != null");
            Assert.AreEqual(1, bet2.SelectedSystems.Count());
            Assert.AreEqual(1, bet2.SelectionRefs.Count());
            Assert.AreEqual(2, bet2.SelectionRefs.ToList()[0].SelectionIndex);

            var json = dto.ToJson();
            Debug.WriteLine(json);
            Assert.IsTrue(!string.IsNullOrEmpty(json));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildSelectionRefWithBankerSetWronglyTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.I1000P).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetBetId("bet-id-" + SR.I1000).SetBetBonus(15000).SetStake(92343, StakeType.Total).AddSelectedSystem(1).AddSelectedSystem(2)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).SetBanker(true).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).SetBanker(false).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildSelectionRefWithDifferentOddsSelectionsTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.I1000P).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetBetId("bet-id-" + SR.I1000).SetBetBonus(15000).SetStake(92343, StakeType.Total).AddSelectedSystem(1)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public void SelectionRefTest()
        {
            ISelectionBuilder selectionBuilder0 = _builderFactory.CreateSelectionBuilder()
                    .SetEventId("11475595")
                    .SetId("live:7/10/*/1")
                    .SetOdds(12000)
                    .SetBanker(true);

            ISelectionBuilder selectionBuilder1 = _builderFactory.CreateSelectionBuilder()
                .SetEventId("11475599")
                .SetId("live:7/10/*/1")
                .SetOdds(11800)
                .SetBanker(false);

            ISelectionBuilder selectionBuilder2 = _builderFactory.CreateSelectionBuilder()
                .SetEventId("11475601")
                .SetId("live:7/10/*/2")
                .SetOdds(17000)
                .SetBanker(false);

            ISelectionBuilder selectionBuilder3 = _builderFactory.CreateSelectionBuilder()
                .SetEventId("11475575")
                .SetId("live:7/10/*/1")
                .SetOdds(11200)
                .SetBanker(false);

            IBetBuilder betBuilder = _builderFactory.CreateBetBuilder()
                .SetBetId("192634_0")
                .SetStake(12345, StakeType.Total)
                .AddSelectedSystem(2);
            betBuilder.AddSelection(selectionBuilder0.Build());
            betBuilder.AddSelection(selectionBuilder1.Build());
            betBuilder.AddSelection(selectionBuilder2.Build());
            betBuilder.AddSelection(selectionBuilder3.Build());

            ITicketBuilder builder = _builderFactory.CreateTicketBuilder()
                .SetTicketId("192634")
                .SetSender(_sender);

            builder.AddBet(betBuilder.Build());

            var ticket = builder.BuildTicket();

            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        public void BuildSelectionRefWithMultipleSameSelectionsCorrectlyTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.I1000P).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetBetId("bet-id-" + SR.I1000).SetBetBonus(15000).SetStake(92343, StakeType.Total).AddSelectedSystem(1).AddSelectedSystem(2)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162701").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).SetBanker(true).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162702").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .Build())
                .AddBet(_builderFactory.CreateBetBuilder().SetBetId("bet-id-" + SR.I1000).SetBetBonus(10000).SetStake(92343, StakeType.Total).AddSelectedSystem(1)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .Build())
                .AddBet(_builderFactory.CreateBetBuilder().SetBetId("bet-id-" + SR.I1000).SetBetBonus(10000).SetStake(92343, StakeType.Total).AddSelectedSystem(1)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(18000).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);

            var dto = new TicketMapper().Map(ticket);
            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Ticket);

            Assert.AreEqual(3, dto.Ticket.Selections.Count());
            Assert.AreEqual(3, dto.Ticket.Bets.Count());

            var bet1 = dto.Ticket.Bets.First();
            Debug.Assert(bet1 != null, "bet1 != null");
            Assert.AreEqual(2, bet1.SelectedSystems.Count());
            Assert.AreEqual(3, bet1.SelectionRefs.Count());
            Assert.AreEqual(0, bet1.SelectionRefs.ToList()[0].SelectionIndex);
            Assert.AreEqual(1, bet1.SelectionRefs.ToList()[1].SelectionIndex);
            Assert.AreEqual(2, bet1.SelectionRefs.ToList()[2].SelectionIndex);

            var bet2 = dto.Ticket.Bets.Last();
            Debug.Assert(bet2 != null, "bet2 != null");
            Assert.AreEqual(1, bet2.SelectedSystems.Count());
            Assert.AreEqual(1, bet2.SelectionRefs.Count());
            Assert.AreEqual(2, bet2.SelectionRefs.ToList()[0].SelectionIndex);

            var json = dto.ToJson();
            Debug.WriteLine(json);
            Assert.IsTrue(!string.IsNullOrEmpty(json));
        }

        [TestMethod]
        public void BuildMultiSelectionWithSameEventIdTicketTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + new Random().Next(10000)).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("111").SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("111").SetIdLcoo(SR.I1000, 2, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("111").SetIdLcoo(SR.I1000, 3, "", "1").SetOdds(SR.I1000P).SetBanker(false).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            var bet = ticket.Bets.First();

            Debug.Assert(bet != null, "bet != null");
            Assert.AreEqual(3, bet.Selections.Count());

            var dto = new TicketMapper().Map(ticket);
            Assert.IsNotNull(dto);
            Assert.AreEqual(3, dto.Ticket.Selections.Count());
            Assert.IsNull(dto.Ticket.Bets.First().SelectionRefs);
            var json = dto.Ticket.ToJson();
            Assert.IsTrue(!json.Contains("selectionRef"));
        }

        [TestMethod]
        public void BetWithoutSumOfWinsProducesCorrectJsonTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.I1000P).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder()
                        .SetBetId("bet-id-" + SR.I1000)
                        .SetBetBonus(15000).SetStake(92343, StakeType.Total)
                        .AddSelectedSystem(1)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);

            var dto = new TicketMapper().Map(ticket);
            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Ticket);

            var json = dto.ToJson();
            Assert.IsTrue(!string.IsNullOrEmpty(json));
            Assert.IsTrue(!json.Contains("sumOfWins"));
        }

        [TestMethod]
        public void BetWithSumOfWinsProducesCorrectJsonTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.I1000P).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder()
                        .SetBetId("bet-id-" + SR.I1000)
                        .SetBetBonus(15000)
                        .SetStake(92343, StakeType.Total)
                        .AddSelectedSystem(1)
                        .SetSumOfWins(1234)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);

            var dto = new TicketMapper().Map(ticket);
            Assert.IsNotNull(dto);
            Assert.IsNotNull(dto.Ticket);

            var json = dto.ToJson();
            Assert.IsTrue(!string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("sumOfWins"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BetWithWrongSelectedSystemTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.I1000).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder()
                        .SetBetId("bet-id-" + SR.I1000)
                        .SetBetBonus(15000).SetStake(92343, StakeType.Total)
                        .AddSelectedSystem(1)
                        .AddSelectedSystem(2)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BetWithZeroSelectedSystemTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.I1000).SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder()
                        .SetBetId("bet-id-" + SR.I1000)
                        .SetBetBonus(15000).SetStake(92343, StakeType.Total)
                        .AddSelectedSystem(0)
                        .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("11162703").SetId("uof:1/sr:sport:1/400/1724?total=4.5").SetOdds(20000).Build())
                        .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsNotNull(ticket.Bets);
            Assert.IsNotNull(ticket.Bets.First().SelectedSystems);
        }

        [TestMethod]
        [DeploymentItem("JSON/ticket-response-customer.json", "JSON")]
        public void BuildTicketResponseFromTicketResponseJson_ReceivedFromCustomerTest()
        {
            var json = FileHelper.ReadFile(DirPath, @"ticket-response-customer.json");
            var dto = TicketResponseDTO.FromJson(json);
            var ticket = new TicketResponseMapper(null).Map(dto, SR.S1000, null, dto.ToJson());
            TicketCompareHelper.Compare(ticket, dto);
            Assert.IsNotNull(ticket.BetDetails);
            Assert.IsNotNull(dto.Result.BetDetails.First().SelectionDetails);
            Assert.IsNull(ticket.BetDetails.First().SelectionDetails);
        }

        [TestMethod]
        public void GetJsonFromTicketTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                    .SetTicketId("T-" + DateTime.Now.Ticks)
                    .SetOddsChange(OddsChangeType.Any)
                    .SetSender(_builderFactory.CreateSenderBuilder()
                            .SetBookmakerId(1)
                            .SetLimitId(1)
                            .SetSenderChannel(SenderChannel.Internet)
                            .SetCurrency("EUR")
                            .SetEndCustomer(IPAddress.Parse("1.2.3.4"), "Customer-" + DateTime.Now.Millisecond, "EN", "device1", 10000)
                            .Build())
                    .AddBet(
                            _builderFactory.CreateBetBuilder()
                                    .SetBetId("Bet-" + DateTime.Now.Ticks)
                                    .AddSelectedSystem(3)
                                    .SetStake(60000, StakeType.Total)
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11050915")
                                                    .SetId("lcoo:20/5/*/1")
                                                    .SetOdds(14100)
                                                    .SetBanker(false)
                                                    .Build())
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11029671")
                                                    .SetId("lcoo:339/5/1.5/2")
                                                    .SetOdds(16800)
                                                    .SetBanker(false)
                                                    .Build())
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11052893")
                                                    .SetId("lcoo:20/5/*/2")
                                                    .SetOdds(16900)
                                                    .SetBanker(false)
                                                    .Build())
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11052531")
                                                    .SetId("lcoo:20/5/*/2")
                                                    .SetOdds(15600)
                                                    .SetBanker(false)
                                                    .Build())
                                    .Build())
                    .AddBet(
                            _builderFactory.CreateBetBuilder()
                                    .SetBetId("Bet-" + DateTime.Now.Ticks)
                                    .AddSelectedSystem(2)
                                    .SetStake(120000, StakeType.Total)
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11029671")
                                                    .SetId("lcoo:339/5/1.5/2")
                                                    .SetOdds(16800)
                                                    .SetBanker(false)
                                                    .Build())
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11052893")
                                                    .SetId("lcoo:20/5/*/2")
                                                    .SetOdds(16900)
                                                    .SetBanker(false)
                                                    .Build())
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11052531")
                                                    .SetId("lcoo:20/5/*/2")
                                                    .SetOdds(15600)
                                                    .SetBanker(false)
                                                    .Build())
                                    .Build())
                    .AddBet(
                            _builderFactory.CreateBetBuilder()
                                    .SetBetId("Bet-" + DateTime.Now.Ticks)
                                    .AddSelectedSystem(1)
                                    .SetStake(80000, StakeType.Total)
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11029671")
                                                    .SetId("lcoo:339/5/1.5/2")
                                                    .SetOdds(16800)
                                                    .SetBanker(false)
                                                    .Build())
                                    .AddSelection(
                                            _builderFactory.CreateSelectionBuilder()
                                                    .SetEventId("11052531")
                                                    .SetId("lcoo:20/5/*/2")
                                                    .SetOdds(15600)
                                                    .SetBanker(false)
                                                    .Build())
                                    .Build())
                    .BuildTicket();
            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = new TicketDTO { Ticket = Ticket.FromJson(json) };
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildMultiBetWithSameSelectionDifferentOddsTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(false).Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(26000).SetBanker(false).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);

            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = new TicketDTO { Ticket = Ticket.FromJson(json) };
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildMultiBetWithSameSelectionDifferentOddsAndSameBankerTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(26000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);

            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = new TicketDTO { Ticket = Ticket.FromJson(json) };
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildMultiBetWithSameSelectionDifferentBankerTest()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(false).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);

            var dto = new TicketMapper().Map(ticket);
            var json = dto.ToJson();

            var newDto = new TicketDTO { Ticket = Ticket.FromJson(json) };
            Assert.IsNotNull(newDto);

            TicketCompareHelper.Compare(ticket, dto);
            TicketCompareHelper.Compare(ticket, newDto);
        }

        [TestMethod]
        public void BuildBetWithDefaultCustomBet()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsNull(ticket.Bets.Single().CustomBet);
            Assert.IsNull(ticket.Bets.Single().CalculationOdds);
        }

        [TestMethod]
        public void BuildBetWithoutCustomBet()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetCustomBet(false).AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsNotNull(ticket.Bets);
            Assert.AreEqual(1, ticket.Bets.Count());
            Assert.IsNotNull(ticket.Bets.Single().CustomBet);
            Assert.AreEqual(true, ticket.Bets.Single().CustomBet.HasValue);
            var customBet = ticket.Bets.Single().CustomBet;
            Assert.IsFalse(customBet != null && customBet.Value);
            Assert.IsNull(ticket.Bets.Single().CalculationOdds);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildBetWithoutCustomBetWithCalculationOdds()
        {
            _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetCustomBet(false).SetCalculationOdds(1000).AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void BuildBetWithCustomBet()
        {
            _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetCustomBet(true).AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();
        }

        [TestMethod]
        public void BuildBetWithCustomBetWithCalculationOdds()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetCustomBet(true).SetCalculationOdds(1000).AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsNotNull(ticket.Bets);
            Assert.AreEqual(1, ticket.Bets.Count());
            Assert.IsNotNull(ticket.Bets.Single().CustomBet);
            Assert.AreEqual(true, ticket.Bets.Single().CustomBet.HasValue);
            var customBet = ticket.Bets.Single().CustomBet;
            Assert.IsTrue(customBet != null && customBet.Value);
            var calculationOdds = ticket.Bets.Single().CalculationOdds;
            Assert.IsNotNull(calculationOdds);
            Assert.AreEqual(1000, calculationOdds.Value);
        }

        [TestMethod]
        public void BuildBetWithoutEntireStake()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsNull(ticket.Bets.Single().EntireStake);
        }

        [TestMethod]
        public void BuildBetWithEntireStake()
        {
            var ticket = _builderFactory.CreateTicketBuilder()
                .SetTicketId("ticket-" + SR.S1000)
                .SetOddsChange(OddsChangeType.Any)
                .SetSender(_sender)
                .AddBet(_builderFactory.CreateBetBuilder().SetEntireStake(12345, StakeType.Total).AddSelectedSystem(1).SetBetId("bet-id-" + SR.I1000).SetBetBonus(SR.I1000).SetStake(92343, StakeType.Total)
                    .AddSelection(_builderFactory.CreateSelectionBuilder().SetEventId("9691139").SetIdLcoo(324, 1, "", "1").SetOdds(16000).SetBanker(true).Build())
                    .Build())
                .BuildTicket();

            Assert.IsNotNull(ticket);
            Assert.IsNotNull(ticket.Bets.Single().EntireStake);
            Assert.AreEqual(12345,ticket.Bets.Single().EntireStake.Value);
            Assert.AreEqual(StakeType.Total, ticket.Bets.Single().EntireStake.Type);
        }
    }
}