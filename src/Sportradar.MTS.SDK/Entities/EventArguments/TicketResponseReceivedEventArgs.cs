/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;

namespace Sportradar.MTS.SDK.Entities.EventArguments
{
    /// <summary>
    /// An event argument used by events raised when a response is received
    /// </summary>
    public class TicketResponseReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a <see cref="ISdkTicket"/> representing received response
        /// </summary>
        public ISdkTicket Response { get; }

        /// <summary>
        /// Gets a value indicating from which type of ticket is this response
        /// </summary>
        public TicketResponseType Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketResponseReceivedEventArgs"/> class
        /// </summary>
        /// <param name="response">a <see cref="ISdkTicket"/> representing the received response</param>
        public TicketResponseReceivedEventArgs(ISdkTicket response)
        {
            Guard.Argument(response, nameof(response)).NotNull();

            Response = response;
            Type = MtsTicketHelper.Convert(response);
        }
    }
}