/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.TicketImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    internal class TicketCancelAckBuilder : ITicketCancelAckBuilder
    {
        private string _ticketId;
        private int _bookmakerId;
        private TicketCancelAckStatus _status;
        private int _code;
        private string _message;

        internal TicketCancelAckBuilder(ISdkConfiguration config)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            _bookmakerId = config.BookmakerId;
        }

        public ITicketCancelAckBuilder SetTicketId(string ticketId)
        {
            _ticketId = ticketId;
            ValidateData(false, true);
            return this;
        }

        public ITicketCancelAckBuilder SetBookmakerId(int bookmakerId)
        {
            _bookmakerId = bookmakerId;
            ValidateData(false, false, true);
            return this;
        }

        public ITicketCancelAckBuilder SetAck(bool markAccepted, int code, string message)
        {
            _status = markAccepted ? TicketCancelAckStatus.Cancelled : TicketCancelAckStatus.NotCancelled;
            _code = code;
            _message = message;
            return this;
        }

        public ITicketCancelAck BuildTicket()
        {
            ValidateData(true);
            return new TicketCancelAck(_ticketId, _bookmakerId, _status, _code, _message);
        }

        private void ValidateData(bool all = false, bool ticketId = false, bool bookmakerId = false)
        {
            if ((all || ticketId) && !TicketHelper.ValidateTicketId(_ticketId))
            {
                throw new ArgumentException("TicketId not valid");
            }
            if ((all || bookmakerId) && _bookmakerId <= 0)
            {
                throw new ArgumentException("BookmakerId not valid.");
            }
        }
    }
}