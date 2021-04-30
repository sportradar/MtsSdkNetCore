/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;
using Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancel;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashout;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCashoutResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketNonSrSettleResponse;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketReofferCancel;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;
using Anonymous = Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.Anonymous;
using Anonymous2 = Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.Anonymous2;

namespace Sportradar.MTS.SDK.Test.Helpers
{
    internal static class TicketCompareHelper
    {
        internal static void Compare(ITicket ticket, TicketDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.Ticket.TicketId);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.Timestamp), dto.Ticket.TimestampUtc);
            Assert.AreEqual(ticket.Version, dto.Ticket.Version);

            Assert.AreEqual(ticket.AltStakeRefId, dto.Ticket.AltStakeRefId);
            Assert.AreEqual(ticket.OddsChange.ToString(), dto.Ticket.OddsChange.ToString());
            Assert.AreEqual(ticket.ReofferId, dto.Ticket.ReofferRefId);
            Assert.AreEqual(ticket.TestSource, dto.Ticket.TestSource);

            Assert.AreEqual(ticket.Sender.BookmakerId, dto.Ticket.Sender.BookmakerId);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.Sender.Channel), dto.Ticket.Sender.Channel);
            Assert.AreEqual(ticket.Sender.Currency.ToUpper(), dto.Ticket.Sender.Currency.ToUpper());
            Assert.AreEqual(ticket.Sender.LimitId, dto.Ticket.Sender.LimitId);
            Assert.AreEqual(ticket.Sender.ShopId, dto.Ticket.Sender.ShopId);
            Assert.AreEqual(ticket.Sender.TerminalId, dto.Ticket.Sender.TerminalId);
            Assert.AreEqual(ticket.Sender.EndCustomer.Id, dto.Ticket.Sender.EndCustomer.Id);
            Assert.AreEqual(ticket.Sender.EndCustomer.Confidence, dto.Ticket.Sender.EndCustomer.Confidence);
            Assert.AreEqual(ticket.Sender.EndCustomer.DeviceId, dto.Ticket.Sender.EndCustomer.DeviceId);
            Assert.AreEqual(ticket.Sender.EndCustomer.Ip, dto.Ticket.Sender.EndCustomer.Ip);
            Assert.AreEqual(ticket.Sender.EndCustomer.LanguageId, dto.Ticket.Sender.EndCustomer.LanguageId);

            for (var i = 0; i < ticket.Bets.Count(); i++)
            {
                Compare(ticket.Bets.ToList()[i], dto.Ticket.Bets.ToList()[i]);
            }

            for (var i = 0; i < ticket.Selections.Count(); i++)
            {
                var ticketSelection = ticket.Selections.ToList()[i];
                var dtoSelection = dto.Ticket.Selections.First(f => f.Id == ticketSelection.Id && f.EventId == ticketSelection.EventId && f.Odds == ticketSelection.Odds);
                Compare(ticketSelection, dtoSelection);
            }
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
            Assert.AreEqual(ticket.TotalCombinations, dto.Ticket.TotalCombinations);
        }

        private static void Compare(IBet bet, Anonymous dto)
        {
            Assert.IsTrue(bet != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(bet.Id, dto.Id);
            Assert.AreEqual(bet.ReofferRefId, dto.ReofferRefId);
            if (bet.SumOfWins == 0)
            {
                Assert.IsNull(dto.SumOfWins);
            }
            else
            {
                Assert.AreEqual(bet.SumOfWins, dto.SumOfWins);
            }
            if (bet.Bonus != null)
            {
                Assert.AreEqual(bet.Bonus.Value, dto.Bonus.Value);
                Assert.AreEqual(MtsTicketHelper.Convert(bet.Bonus.Type), dto.Bonus.Type);
                Assert.AreEqual(MtsTicketHelper.Convert(bet.Bonus.Mode), dto.Bonus.Mode);
            }

            Assert.AreEqual(bet.Stake.Value, dto.Stake.Value);

            if (bet.Stake.Type.HasValue)
            {
                Assert.AreEqual(MtsTicketHelper.ConvertStakeType(bet.Stake.Type.Value), dto.Stake.Type);
            }

            Assert.AreEqual(bet.SelectedSystems.Count(), dto.SelectedSystems.Count());
            for (var i = 0; i < bet.SelectedSystems.Count(); i++)
            {
                Assert.AreEqual(bet.SelectedSystems.ToList()[i], dto.SelectedSystems.ToList()[i]);
            }
        }

        private static void Compare(ISelection sel, Anonymous2 dto)
        {
            Assert.IsTrue(sel != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(sel.Id, dto.Id);
            Assert.AreEqual(sel.EventId, dto.EventId);
            Assert.AreEqual(sel.Odds, dto.Odds);
        }

        internal static void Compare(ITicketCancel ticket, TicketCancelDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.Cancel.TicketId);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.Timestamp), dto.Cancel.TimestampUtc);
            Assert.AreEqual(ticket.Version, dto.Cancel.Version);

            Assert.AreEqual(ticket.BookmakerId, dto.Cancel.Sender.BookmakerId);
            Assert.AreEqual((int)ticket.Code, dto.Cancel.Code);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketAck ticket, TicketAckDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.TicketId);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.Timestamp), dto.TimestampUtc);
            Assert.AreEqual(ticket.Version, dto.Version);

            Assert.AreEqual(ticket.BookmakerId, dto.Sender.BookmakerId);
            Assert.AreEqual(ticket.Code, dto.Code);
            Assert.AreEqual(ticket.Message, dto.Message);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.TicketStatus), dto.TicketStatus);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketCancelAck ticket, TicketCancelAckDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.TicketId);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.Timestamp), dto.TimestampUtc);
            Assert.AreEqual(ticket.Version, dto.Version);

            Assert.AreEqual(ticket.BookmakerId, dto.Sender.BookmakerId);
            Assert.AreEqual(ticket.Code, dto.Code);
            Assert.AreEqual(ticket.Message, dto.Message);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.TicketCancelStatus), dto.TicketCancelStatus);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketResponse ticket, TicketResponseDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.Result.TicketId);
            Assert.AreEqual(ticket.Status, MtsTicketHelper.Convert(dto.Result.Status));
            Assert.AreEqual(ticket.Signature, dto.Signature);
            Assert.AreEqual(ticket.ExchangeRate, dto.ExchangeRate);
            Assert.AreEqual(ticket.Version, dto.Version);

            Assert.AreEqual(ticket.Reason.Code, dto.Result.Reason.Code);
            Assert.AreEqual(ticket.Reason.Message, dto.Result.Reason.Message);

            if (ticket.BetDetails == null || !ticket.BetDetails.Any())
            {
                return;
            }

            for (var i = 0; i < ticket.BetDetails.Count(); i++)
            {
                var bd1 = ticket.BetDetails.ToList()[i];
                var bd2 = dto.Result.BetDetails.ToList()[i];
                Compare(bd1.Reason, bd2.Reason);
                Assert.AreEqual(bd1.BetId, bd2.BetId);
                if (bd1.AlternativeStake != null)
                {
                    Assert.AreEqual(bd1.AlternativeStake.Stake, bd2.AlternativeStake.Stake);
                }
                if (bd1.Reoffer != null)
                {
                    Assert.AreEqual(bd1.Reoffer.Stake, bd2.Reoffer.Stake);
                    Assert.AreEqual(bd1.Reoffer.Type, MtsTicketHelper.Convert(bd2.Reoffer.Type));
                }
                if (bd2.SelectionDetails != null && bd2.SelectionDetails.Any())
                {
                    for (var j = 0; j < bd2.SelectionDetails.Count(); j++)
                    {
                        Compare(bd1.SelectionDetails.ToList()[i].Reason, bd2.SelectionDetails.ToList()[i].Reason);
                        Assert.AreEqual(bd1.SelectionDetails.ToList()[i].SelectionIndex, bd2.SelectionDetails.ToList()[i].SelectionIndex);
                        if (bd2.SelectionDetails.ToList()[i].RejectionInfo != null)
                        {
                            Compare(bd1.SelectionDetails.ToList()[i].RejectionInfo, bd2.SelectionDetails.ToList()[i].RejectionInfo);
                        }
                    }
                }
            }
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketCancelResponse ticket, TicketCancelResponseDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.Result.TicketId);
            Assert.AreEqual(ticket.Status, MtsTicketHelper.Convert(dto.Result.Status));
            Assert.AreEqual(ticket.Signature, dto.Signature);
            Assert.AreEqual(ticket.Version, dto.Version);

            Assert.AreEqual(ticket.Reason.Code, dto.Result.Reason.Code);
            Assert.AreEqual(ticket.Reason.Message, dto.Result.Reason.Message);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketReofferCancel ticket, TicketReofferCancelDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.TicketId);
            Assert.AreEqual(ticket.BookmakerId, dto.Sender.BookmakerId);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.Timestamp), dto.TimestampUtc);
            Assert.AreEqual(ticket.Version, dto.Version);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketCashout ticket, TicketCashoutDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.TicketId);
            Assert.AreEqual(ticket.BookmakerId, dto.Sender.BookmakerId);
            Assert.AreEqual(MtsTicketHelper.Convert(ticket.Timestamp), dto.TimestampUtc);
            Assert.AreEqual(ticket.Version, dto.Version);
            Assert.AreEqual(ticket.CashoutStake, dto.CashoutStake);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketCashoutResponse ticket, TicketCashoutResponseDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.Result.TicketId);
            Assert.AreEqual(ticket.Signature, dto.Signature);
            Assert.AreEqual(ticket.Version, dto.Version);

            Assert.AreEqual(ticket.Status, MtsTicketHelper.Convert(dto.Result.Status));
            Assert.AreEqual(ticket.Reason.Code, dto.Result.Reason.Code);
            Assert.AreEqual(ticket.Reason.Message, dto.Result.Reason.Message);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        internal static void Compare(ITicketNonSrSettleResponse ticket, TicketNonSrSettleResponseDTO dto)
        {
            Assert.IsTrue(ticket != null);
            Assert.IsTrue(dto != null);

            Assert.AreEqual(ticket.TicketId, dto.Result.TicketId);
            Assert.AreEqual(ticket.Signature, dto.Signature);
            Assert.AreEqual(ticket.Version, dto.Version);

            Assert.AreEqual(ticket.Status, MtsTicketHelper.Convert(dto.Result.Status));
            Assert.AreEqual(ticket.Reason.Code, dto.Result.Reason.Code);
            Assert.AreEqual(ticket.Reason.Message, dto.Result.Reason.Message);
            Assert.IsFalse(string.IsNullOrEmpty(ticket.CorrelationId));
        }

        private static void Compare(IResponseReason reason, SDK.Entities.Internal.Dto.TicketResponse.Reason dto)
        {
            Assert.AreEqual(reason.Code, dto.Code);
            Assert.AreEqual(reason.Message, dto.Message);
        }

        private static void Compare(IRejectionInfo rejection, RejectionInfo dto)
        {
            Assert.AreEqual(rejection.Id, dto.Id);
            Assert.AreEqual(rejection.EventId, dto.EventId);
            Assert.AreEqual(rejection.Odds, dto.Odds);
        }

        internal static void Compare(ITicket orgTicket, ITicket newTicket, bool isReoffer, bool isAlt)
        {
            Assert.IsNotNull(orgTicket);
            Assert.IsNotNull(newTicket);

            Assert.AreEqual(orgTicket.OddsChange, newTicket.OddsChange);
            Assert.AreEqual(orgTicket.Sender, newTicket.Sender);
            for (var i = 0; i < orgTicket.Selections.Count(); i++)
            {
                Assert.AreEqual(orgTicket.Selections.ElementAt(i), newTicket.Selections.ElementAt(i));
            }
            Assert.AreEqual(orgTicket.TestSource, newTicket.TestSource);
            Assert.AreEqual(orgTicket.Version, newTicket.Version);
            Assert.AreNotEqual(orgTicket.Timestamp, newTicket.Timestamp);

            if (isReoffer)
            {
                Assert.AreNotEqual(orgTicket.TicketId, newTicket.TicketId);
                Assert.AreEqual(orgTicket.TicketId, newTicket.ReofferId);
                Assert.IsTrue(string.IsNullOrEmpty(newTicket.AltStakeRefId));
            }
            else if (isAlt)
            {
                Assert.AreNotEqual(orgTicket.TicketId, newTicket.TicketId);
                Assert.AreEqual(orgTicket.TicketId, newTicket.AltStakeRefId);
                Assert.IsTrue(string.IsNullOrEmpty(newTicket.ReofferId));
            }
            else
            {
                Assert.AreEqual(orgTicket.TicketId, newTicket.TicketId);
                Assert.AreEqual(orgTicket.ReofferId, newTicket.ReofferId);
                Assert.AreEqual(orgTicket.AltStakeRefId, newTicket.AltStakeRefId);
            }

            foreach (var ticketBet in orgTicket.Bets)
            {
                if (isReoffer || isAlt)
                {
                    Compare(ticketBet, newTicket.Bets.First(f => f.ReofferRefId == ticketBet.Id), true);
                }
                else
                {
                    Compare(ticketBet, newTicket.Bets.First(f => f.Id == ticketBet.Id), false);
                }
            }
            Assert.IsFalse(string.IsNullOrEmpty(orgTicket.CorrelationId));
            Assert.IsFalse(string.IsNullOrEmpty(newTicket.CorrelationId));
            Assert.AreNotEqual(orgTicket.CorrelationId, newTicket.CorrelationId);
        }

        internal static void Compare(IBet bet, IBet newBet, bool isReoffer)
        {
            Assert.IsNotNull(bet);
            Assert.IsNotNull(newBet);

            if (bet.Bonus != null)
            {
                Assert.AreEqual(bet.Bonus.Value, newBet.Bonus.Value);
                Assert.AreEqual(bet.Bonus.Mode, newBet.Bonus.Mode);
                Assert.AreEqual(bet.Bonus.Type, newBet.Bonus.Type);
            }
            if (bet.SelectedSystems != null)
            {
                Assert.AreEqual(bet.SelectedSystems.Count(), newBet.SelectedSystems.Count());
                Assert.IsTrue(bet.SelectedSystems.All(a => newBet.SelectedSystems.Contains(a)));
            }
            Assert.AreEqual(bet.SumOfWins, newBet.SumOfWins);

            if (isReoffer)
            {
                Assert.AreNotEqual(bet.Id, newBet.Id);
                Assert.AreEqual(bet.Id, newBet.ReofferRefId);
            }
            else
            {
                Assert.AreEqual(bet.Id, newBet.Id);
                Assert.AreEqual(bet.ReofferRefId, newBet.ReofferRefId);
                Assert.AreEqual(bet.Stake, newBet.Stake);
            }
        }
    }
}