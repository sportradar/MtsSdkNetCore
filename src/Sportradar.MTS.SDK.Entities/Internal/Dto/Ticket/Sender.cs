/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    public partial class Sender
    {
        public Sender()
        { }

        public Sender(EndCustomer customer, SenderChannel channel, string currency, int limitId, int bookmakerId, string shopId, string terminalId)
        {
            EndCustomer = customer;
            Channel = channel;
            Currency = currency;
            LimitId = limitId;
            BookmakerId = bookmakerId;
            ShopId = shopId;
            TerminalId = terminalId;
        }

        public Sender(ISender sender)
        {
            Guard.Argument(sender).NotNull();

            if (sender.EndCustomer != null)
            {
                EndCustomer = new EndCustomer(sender.EndCustomer);
            }
            Channel = MtsTicketHelper.Convert(sender.Channel);
            Currency = sender.Currency;
            LimitId = sender.LimitId;
            BookmakerId = sender.BookmakerId;
            ShopId = sender.ShopId;
            TerminalId = sender.TerminalId;
        }
    }
}