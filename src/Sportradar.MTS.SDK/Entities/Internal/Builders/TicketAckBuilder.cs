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
    internal class TicketAckBuilder : ITicketAckBuilder
    {
        private string _ticketId;
        private int _bookmakerId;
        private TicketAckStatus _status;
        private int _code;
        private string _message;

        internal TicketAckBuilder(ISdkConfiguration config)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            _bookmakerId = config.BookmakerId;
        }

        public ITicketAckBuilder SetTicketId(string ticketId)
        {
            _ticketId = ticketId;
            ValidateData(false, true);
            return this;
        }

        public ITicketAckBuilder SetBookmakerId(int bookmakerId)
        {
            _bookmakerId = bookmakerId;
            ValidateData(false, false, true);
            return this;
        }

        public ITicketAckBuilder SetAck(bool markAccepted, int code, string message)
        {
            _status = markAccepted ? TicketAckStatus.Accepted : TicketAckStatus.Rejected;
            _code = code;
            _message = message;
            return this;
        }

        public ITicketAck BuildTicket()
        {
            ValidateData(true);
            return new TicketAck(_ticketId, _bookmakerId, _status, _code, _message);
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