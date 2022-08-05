/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Builders;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashoutResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettleResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;
using AlternativeStake = Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse.AlternativeStake;
using Reason = Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse.Reason;
using Result = Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse.Result;
using SR = Sportradar.MTS.SDK.Test.Helpers.StaticRandom;
using Status = Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse.Status;

namespace Sportradar.MTS.SDK.Test.Helpers
{
    public static class TicketBuilderHelper
    {
        private static readonly IBuilderFactory BuilderFactory = new BuilderFactoryHelper().BuilderFactory;

        public static ISdkConfiguration GetSdkConfiguration()
        {
            return MtsSdk.CreateConfigurationBuilder()
                         .SetUsername("username")
                         .SetPassword("password")
                         .SetHost("127.0.0.1")
                         .SetVirtualHost("/test")
                         .SetUseSsl(true)
                         .SetLimitId(111)
                         .SetBookmakerId(222)
                         .SetAccessToken("aaa")
                         .SetCurrency("mBTC")
                         .SetNode(10)
                         .SetSenderChannel(SenderChannel.Mobile)
                         .Build();
        }

        public static ITicket GetTicket(string ticketId = null, int bookmakerId = 0, int betCount = 1, int selectionCount = 1)
        {
            var tb = BuilderFactory.CreateTicketBuilder();
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            if (bookmakerId < 1)
            {
                bookmakerId = SR.I1000P;
            }
            if (betCount < 1)
            {
                betCount = 1;
            }
            if (selectionCount < 1)
            {
                selectionCount = 1;
            }
            for (var i = 0; i < betCount; i++)
            {
                var betBuilder = BuilderFactory.CreateBetBuilder();
                for (var j = 0; j < selectionCount; j++)
                {
                    betBuilder.AddSelection(BuilderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());
                }

                var bet = betBuilder
                    .AddSelectedSystem(1)
                    .SetBetId("bet-id-" + SR.I1000)
                    .SetBetBonus(SR.I1000)
                    .SetStake(92343, StakeType.Total)
                    .Build();
                tb.AddBet(bet);
            }
            var ticket = tb.SetTicketId(ticketId).SetOddsChange(OddsChangeType.Any)
                           .SetSender(BuilderFactory.CreateSenderBuilder().SetBookmakerId(bookmakerId).SetLimitId(SR.I100).SetCurrency("EUR").SetSenderChannel(SenderChannel.Internet)
                                        .SetEndCustomer(BuilderFactory.CreateEndCustomerBuilder().SetId("customer-client-" + SR.I1000).SetConfidence(SR.I1000P).SetIp(IPAddress.Loopback).SetLanguageId("en").SetDeviceId(SR.S1000).Build())
                           .Build())
                           .SetTotalCombinations(tb.GetBets().Count())
                           .BuildTicket();
            return ticket;
        }

        public static ITicketBuilder GetTicketBuilder(ISender sender = null, IEndCustomer endCustomer = null, int selectionCount = 0, int betCount = 0)
        {
            var tb = BuilderFactory.CreateTicketBuilder();
            tb.SetTicketId("ticket-" + SR.I1000P);
            if (sender != null)
            {
                tb.SetSender(sender);
            }
            if (endCustomer != null)
            {
                tb.SetSender(BuilderFactory.CreateSenderBuilder().SetBookmakerId(SR.I1000P).SetLimitId(SR.I100)
                                .SetCurrency("EUR").SetSenderChannel(SenderChannel.Internet).SetEndCustomer(endCustomer).Build());
            }

            for (var i = 0; i < betCount; i++)
            {
                var betBuilder = BuilderFactory.CreateBetBuilder();
                for (var j = 0; j < selectionCount; j++)
                {
                    betBuilder.AddSelection(BuilderFactory.CreateSelectionBuilder().SetEventId(SR.S1000P).SetIdLcoo(SR.I1000, 1, "", "1").SetOdds(SR.I1000P).SetBanker(SR.I100 > 90).Build());
                }

                var bet = betBuilder
                    .AddSelectedSystem(1)
                    .SetBetId("bet-id-" + SR.I1000)
                    .SetBetBonus(SR.I1000)
                    .SetStake(SR.I1000P, StakeType.Total)
                    .Build();
                tb.AddBet(bet);
                tb.SetTotalCombinations(tb.GetBets().Count());
            }
            return tb;
        }

        public static ITicketCancel GetTicketCancel(string ticketId = null)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return BuilderFactory.CreateTicketCancelBuilder().SetTicketId(ticketId).SetBookmakerId(SR.I1000).SetCode(TicketCancellationReason.BookmakerBackofficeTriggered).BuildTicket();
        }

        public static ITicketAck GetTicketAck(string ticketId = null, bool accepted = true)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return new TicketAckBuilder(GetSdkConfiguration())
                            .SetTicketId(ticketId).SetBookmakerId(SR.I1000)
                            .SetAck(accepted, (int)TicketCancellationReason.BookmakerTechnicalIssue, $"{TicketCancellationReason.BookmakerBackofficeTriggered}")
                        .BuildTicket();
        }

        public static ITicketCancelAck GetTicketCancelAck(string ticketId = null, bool accepted = true)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return new TicketCancelAckBuilder(GetSdkConfiguration())
                            .SetTicketId(ticketId).SetBookmakerId(SR.I1000)
                            .SetAck(accepted, (int)TicketCancellationReason.BookmakerBackofficeTriggered, $"{TicketCancellationReason.BookmakerBackofficeTriggered}")
                        .BuildTicket();
        }

        internal static TicketResponseDTO GetTicketResponse()
        {
            return new TicketResponseDTO
            {
                ExchangeRate = SR.I1000P,
                Signature = SR.S1000,
                Version = "2.0",
                Result = new Result
                {
                    BetDetails = new List<Anonymous2>
                    {
                        GetResponseBetDetail(null, SR.B, SR.B)
                    },
                    Reason = new Reason
                    {
                        Code = SR.I1000,
                        Message = "message " + SR.I1000
                    },
                    Status = Status.Rejected,
                    TicketId = "ticket-id-" + SR.S1000
                }
            };
        }

        internal static TicketResponseDTO GetTicketResponse(ITicket ticket, Status status, bool addReoffer, bool addAlt)
        {
            var betDetails = ticket.Bets.Select(ticketBet => GetResponseBetDetail(ticketBet.Id, addAlt, addReoffer)).ToList();

            return new TicketResponseDTO
            {
                ExchangeRate = SR.I1000P,
                Signature = SR.S1000,
                Version = "2.0",
                Result = new Result
                {
                    BetDetails = betDetails,
                    Reason = new Reason
                    {
                        Code = SR.I1000,
                        Message = "message " + SR.I1000
                    },
                    Status = status,
                    TicketId = ticket.TicketId
                }
            };
        }

        internal static TicketCancelResponseDTO GetTicketCancelResponse()
        {
            return new TicketCancelResponseDTO
            {
                Signature = SR.S1000,
                Version = "2.0",
                Result = new SDK.Entities.Internal.Dto.TicketCancelResponse.Result
                {
                    TicketId =  "ticket-id-" + SR.S1000,
                    Status = SDK.Entities.Internal.Dto.TicketCancelResponse.Status.Not_cancelled,
                    Reason = new SDK.Entities.Internal.Dto.TicketCancelResponse.Reason
                    {
                        Code = SR.I1000,
                        Message = "message " + SR.S1000
                    }
                }
            };
        }

        public static ITicketReofferCancel GetTicketReofferCancel(string ticketId = null)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return BuilderFactory.CreateTicketReofferCancelBuilder().SetTicketId(ticketId).SetBookmakerId(SR.I1000).BuildTicket();
        }

        public static ITicketCashout GetTicketCashout(string ticketId = null)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return BuilderFactory.CreateTicketCashoutBuilder().SetTicketId(ticketId).SetBookmakerId(SR.I1000).SetCashoutStake(SR.I1000P).BuildTicket();
        }

        internal static TicketCashoutResponseDTO GetTicketCashoutResponse(string ticketId, SDK.Entities.Internal.Dto.TicketCashoutResponse.Status status)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return new TicketCashoutResponseDTO
            {
                Signature = SR.S1000,
                Version = "2.0",
                Result = new SDK.Entities.Internal.Dto.TicketCashoutResponse.Result
                {
                    Reason = new SDK.Entities.Internal.Dto.TicketCashoutResponse.Reason
                    {
                        Code = SR.I1000,
                        Message = "message " + SR.I1000
                    },
                    Status = status,
                    TicketId = ticketId
                }
            };
        }

        public static ITicketNonSrSettle GetTicketNonSrSettle(string ticketId = null)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return BuilderFactory.CreateTicketNonSrSettleBuilder().SetTicketId(ticketId).SetBookmakerId(SR.I1000).SetNonSrSettleStake(SR.I1000P).BuildTicket();
        }

        internal static TicketNonSrSettleResponseDTO GetTicketNonSrSettleResponse(string ticketId, SDK.Entities.Internal.Dto.TicketNonSrSettleResponse.Status status)
        {
            if (string.IsNullOrEmpty(ticketId))
            {
                ticketId = "ticket-" + SR.I1000P;
            }
            return new TicketNonSrSettleResponseDTO
            {
                Signature = SR.S1000,
                Version = "2.4",
                Result = new SDK.Entities.Internal.Dto.TicketNonSrSettleResponse.Result
                {
                    Reason = new SDK.Entities.Internal.Dto.TicketNonSrSettleResponse.Reason
                    {
                        Code = SR.I1000,
                        Message = "message " + SR.I1000
                    },
                    Status = status,
                    TicketId = ticketId
                }
            };
        }

        private static Anonymous2 GetResponseBetDetail(string betId, bool reoffer, bool alt)
        {
            var reofferType = SR.B ? ReofferType.Auto : ReofferType.Manual;
            return new Anonymous2
            {
                BetId = string.IsNullOrEmpty(betId) ? "bet-id-" + SR.S1000 : betId,
                Reason = GetReason(),
                AlternativeStake = alt ? null : new AlternativeStake
                    {
                        Stake = SR.I1000P
                    },
                Reoffer = reoffer ? null : new Reoffer
                    {
                        Stake = SR.I1000P,
                        Type = reofferType
                    },
                SelectionDetails = new List<Anonymous3>
                {
                    new Anonymous3
                    {
                        SelectionIndex = SR.II(50),
                        Reason = GetReason(),
                        RejectionInfo = GetRejectionInfo()
                    }
                }
            };
        }

        private static Reason GetReason()
        {
            return new Reason
            {
                Code = SR.I1000,
                Message = "message " + SR.I1000
            };
        }

        private static RejectionInfo GetRejectionInfo()
        {
            return new RejectionInfo
            {
                Id = SR.S1000,
                EventId = SR.S1000,
                Odds = SR.I1000
            };
        }
    }
}