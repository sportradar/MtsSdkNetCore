/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.ComponentModel;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketCancelAck;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;
using SenderChannel = Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.SenderChannel;
using StakeType = Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.StakeType;
using FreeStakeDescription = Sportradar.MTS.SDK.Entities.Enums.FreeStakeDescription;
using FreeStakePaidAs = Sportradar.MTS.SDK.Entities.Enums.FreeStakePaidAs;
using FreeStakeType = Sportradar.MTS.SDK.Entities.Enums.FreeStakeType;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto
{
    internal static class MtsTicketHelper
    {
        public static TicketAckDTOTicketStatus Convert(TicketAckStatus status)
        {
            if (status == TicketAckStatus.Accepted)
            {
                return TicketAckDTOTicketStatus.Accepted;
            }

            if (status == TicketAckStatus.Rejected)
            {
                return TicketAckDTOTicketStatus.Rejected;
            }
            throw new InvalidEnumArgumentException($"Invalid TicketAckStatus value: {status}.");
        }

        public static TicketCancelAckDTOTicketCancelStatus Convert(TicketCancelAckStatus status)
        {
            if (status == TicketCancelAckStatus.Cancelled)
            {
                return TicketCancelAckDTOTicketCancelStatus.Cancelled;
            }

            if (status == TicketCancelAckStatus.NotCancelled)
            {
                return TicketCancelAckDTOTicketCancelStatus.Not_cancelled;
            }
            throw new InvalidEnumArgumentException($"Invalid TicketCancelAckStatus value: {status}.");
        }

        public static SenderChannel Convert(Entities.Enums.SenderChannel channel)
        {
            switch (channel)
            {
                case Entities.Enums.SenderChannel.CallCentre:
                    return SenderChannel.CallCentre;
                case Entities.Enums.SenderChannel.Internet:
                    return SenderChannel.Internet;
                case Entities.Enums.SenderChannel.Mobile:
                    return SenderChannel.Mobile;
                case Entities.Enums.SenderChannel.Retail:
                    return SenderChannel.Retail;
                case Entities.Enums.SenderChannel.Sms:
                    return SenderChannel.Sms;
                case Entities.Enums.SenderChannel.Terminal:
                    return SenderChannel.Terminal;
                case Entities.Enums.SenderChannel.TvApp:
                    return SenderChannel.TvApp;
                case Entities.Enums.SenderChannel.Agent:
                    return SenderChannel.Agent;
                default:
                    throw new InvalidEnumArgumentException($"Invalid Entities.Enums.SenderChannel value: {channel}.");
            }
        }

        public static SenderChannel Convert(string channel)
        {
            channel = channel?.ToLower() ?? string.Empty;

            switch (channel)
            {
                case "callcenter":
                    return SenderChannel.CallCentre;
                case "internet":
                    return SenderChannel.Internet;
                case "mobile":
                    return SenderChannel.Mobile;
                case "phone":
                    return SenderChannel.Phone;
                case "retail":
                    return SenderChannel.Retail;
                case "sms":
                    return SenderChannel.Sms;
                case "terminal":
                    return SenderChannel.Terminal;
                case "tvapp":
                    return SenderChannel.TvApp;
                case "agent":
                    return SenderChannel.Agent;
                default:
                    throw new InvalidEnumArgumentException($"Invalid channel value: {channel}.");
            }
        }

        public static TicketOddsChange Convert(OddsChangeType type)
        {
            switch (type)
            {
                case OddsChangeType.Higher:
                    return TicketOddsChange.Higher;
                case OddsChangeType.Any:
                    return TicketOddsChange.Any;
                case OddsChangeType.None:
                    return TicketOddsChange.None;
            }
            throw new InvalidEnumArgumentException($"Invalid OddsChangeType value: {type}.");
        }

        public static long Convert(DateTime dateTime)
        {
            return TicketHelper.DateTimeToUnixTime(dateTime);
        }

        public static Entities.Enums.StakeType Convert(StakeType type)
        {
            if (type == StakeType.Total)
            {
                return Entities.Enums.StakeType.Total;
            }

            if (type == StakeType.Unit)
            {
                return Entities.Enums.StakeType.Unit;
            }
            throw new InvalidEnumArgumentException($"Invalid Entities.Enums.StakeType value: {type}.");
        }

        public static BonusType Convert(BetBonusType type)
        {
            if (type != BetBonusType.Total)
            {
                throw new InvalidEnumArgumentException($"Invalid BetBonusType value: {type}.");
            }
            return BonusType.Total;
        }

        public static BonusMode Convert(BetBonusMode type)
        {
            switch (type)
            {
                case BetBonusMode.All:
                    return BonusMode.All;
            }
            throw new InvalidEnumArgumentException($"Invalid BetBonusMode value: {type}.");
        }

        public static BonusDescription Convert(BetBonusDescription description)
        {
            switch (description)
            {
                case BetBonusDescription.AccaBonus:
                    return BonusDescription.AccaBonus;
                case BetBonusDescription.OddsBooster:
                    return BonusDescription.OddsBooster;
                case BetBonusDescription.Other:
                    return BonusDescription.Other;
            }
            throw new InvalidEnumArgumentException($"Invalid BetBonusDescription value: {description}.");
        }

        public static BonusPaidAs Convert(BetBonusPaidAs paidAs)
        {
            switch (paidAs)
            {
                case BetBonusPaidAs.Cash:
                    return BonusPaidAs.Cash;
                case BetBonusPaidAs.FreeBet:
                    return BonusPaidAs.FreeBet;
            }
            throw new InvalidEnumArgumentException($"Invalid BonusPaidAs value: {paidAs}.");
        }

        public static Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeDescription Convert(FreeStakeDescription description)
        {
            switch (description)
            {
                case FreeStakeDescription.FreeBet:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeDescription.FreeBet;
                case FreeStakeDescription.MoneyBack:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeDescription.MoneyBack;
                case FreeStakeDescription.Rollover:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeDescription.Rollover;
                case FreeStakeDescription.PartialFreeBet:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeDescription.PartialFreeBet;
                case FreeStakeDescription.OddsBooster:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeDescription.OddsBooster;
                case FreeStakeDescription.Other:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeDescription.Other;
            }
            throw new InvalidEnumArgumentException($"Invalid FreeStakeDescription value: {description}.");
        }

        public static Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakePaidAs Convert(FreeStakePaidAs paidAs)
        {
            switch (paidAs)
            {
                case FreeStakePaidAs.Cash:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakePaidAs.Cash;
                case FreeStakePaidAs.FreeBet:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakePaidAs.FreeBet;
            }
            throw new InvalidEnumArgumentException($"Invalid FreeStakePaidAs value: {paidAs}.");
        }

        public static Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeType Convert(FreeStakeType type)
        {
            switch (type)
            {
                case FreeStakeType.Total:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeType.Total;
                case FreeStakeType.Unit:
                    return Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket.FreeStakeType.Unit;
            }
            throw new InvalidEnumArgumentException($"Invalid FreeStakeType value: {type}.");
        }

        public static TicketAcceptance Convert(Status status)
        {
            if (status == Status.Accepted)
            {
                return TicketAcceptance.Accepted;
            }

            if (status == Status.Rejected)
            {
                return TicketAcceptance.Rejected;
            }
            throw new InvalidEnumArgumentException($"Invalid TicketResponse. Status value: {status}.");
        }

        public static TicketCancelAcceptance Convert(TicketCancelResponse.Status status)
        {
            if (status == TicketCancelResponse.Status.Cancelled)
            {
                return TicketCancelAcceptance.Cancelled;
            }

            if (status == TicketCancelResponse.Status.Not_cancelled)
            {
                return TicketCancelAcceptance.NotCancelled;
            }
            throw new InvalidEnumArgumentException($"Invalid TicketCancelResponse. Status value: {status}.");
        }

        public static BetReofferType Convert(ReofferType type)
        {
            if (type == ReofferType.Auto)
            {
                return BetReofferType.Auto;
            }

            if (type == ReofferType.Manual)
            {
                return BetReofferType.Manual;
            }
            throw new InvalidEnumArgumentException($"Invalid ReofferType value: {type}.");
        }

        public static CashoutAcceptance Convert(TicketCashoutResponse.Status status)
        {
            if (status == TicketCashoutResponse.Status.Accepted)
            {
                return CashoutAcceptance.Accepted;
            }

            if (status == TicketCashoutResponse.Status.Rejected)
            {
                return CashoutAcceptance.Rejected;
            }
            throw new InvalidEnumArgumentException($"Invalid TicketCashoutResponse. Status value: {status}.");
        }

        public static NonSrSettleAcceptance Convert(TicketNonSrSettleResponse.Status status)
        {
            if (status == TicketNonSrSettleResponse.Status.Accepted)
            {
                return NonSrSettleAcceptance.Accepted;
            }

            if (status == TicketNonSrSettleResponse.Status.Rejected)
            {
                return NonSrSettleAcceptance.Rejected;
            }
            throw new InvalidEnumArgumentException($"Invalid TicketNonSrSettleResponse. Status value: {status}.");
        }

        public static TicketResponseType Convert(ISdkTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket), "Ticket must not be null.");
            }
            if (ticket is ITicketResponse)
            {
                return TicketResponseType.Ticket;
            }
            if(ticket is ITicketCancelResponse)
            {
                return TicketResponseType.TicketCancel;
            }
            if (ticket is ITicketCashoutResponse)
            {
                return TicketResponseType.TicketCashout;
            }
            if (ticket is ITicketNonSrSettleResponse)
            {
                return TicketResponseType.TicketNonSrSettle;
            }
            throw new InvalidEnumArgumentException($"Invalid ticket type. Ticket type: {ticket.GetType()}.");
        }

        public static StakeType ConvertStakeType(Entities.Enums.StakeType type)
        {
            if (type == Entities.Enums.StakeType.Total)
            {
                return StakeType.Total;
            }

            if (type == Entities.Enums.StakeType.Unit)
            {
                return StakeType.Unit;
            }
            throw new InvalidEnumArgumentException($"Invalid Entities.Enums.StakeType value: {type}.");
        }

        public static EntireStakeType ConvertEntireStakeType(Entities.Enums.StakeType type)
        {
            if (type == Entities.Enums.StakeType.Total)
            {
                return EntireStakeType.Total;
            }

            if (type == Entities.Enums.StakeType.Unit)
            {
                return EntireStakeType.Unit;
            }
            throw new InvalidEnumArgumentException($"Invalid Entities.Enums.StakeType value: {type}.");
        }

        public static SourceType ConvertSourceType(string sourceType)
        {
            if(string.IsNullOrEmpty(sourceType))
            {
                throw new ArgumentNullException(nameof(sourceType), "Missing source type");
            }
            sourceType = sourceType.ToLower().Trim();
            switch (sourceType)
            {
                case "shop":
                    return SourceType.Shop;
                case "terminal":
                    return SourceType.Terminal;
                case "customer":
                    return SourceType.Customer;
                case "bookmaker":
                    return SourceType.Bookmaker;
                case "sub_bookmaker":
                case "subbookmaker":
                    return SourceType.SubBookmaker;
                case "distribution_channel":
                case "distributionchannel":
                    return SourceType.DistributionChannel;
            }

            throw new InvalidEnumArgumentException($"Invalid source type. Source type: {sourceType}.");
        }

        public static bool TryConvertSourceType(string sourceTypeString, out SourceType sourceType)
        { 
            sourceType = SourceType.Customer;
            try
            {
                sourceType = ConvertSourceType(sourceTypeString);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}