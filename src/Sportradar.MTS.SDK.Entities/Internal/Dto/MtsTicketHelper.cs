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

namespace Sportradar.MTS.SDK.Entities.Internal.Dto
{
    public static class MtsTicketHelper
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
                //case BetBonusMode.Any:
                //    return BonusMode.Any;
                case BetBonusMode.All:
                    return BonusMode.All;
                //case BetBonusMode.Proportional:
                //    return BonusMode.Proportional;
            }
            throw new InvalidEnumArgumentException($"Invalid BetBonusMode value: {type}.");
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
    }
}