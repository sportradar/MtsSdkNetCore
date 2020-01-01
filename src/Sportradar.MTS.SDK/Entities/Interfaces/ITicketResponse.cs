/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for ticket submission response
    /// </summary>
    public interface ITicketResponse : ISdkTicket, IAcknowledgeable
    {
        /// <summary>
        /// Gets the status of the ticket submission
        /// </summary>
        TicketAcceptance Status { get; }

        /// <summary>
        /// Gets the response reason
        /// </summary>
        IResponseReason Reason { get; }

        /// <summary>
        /// Gets the bet details
        /// </summary>
        IEnumerable<IBetDetail> BetDetails { get; }

        /// <summary>
        /// Gets the response signature/hash (previous BetAcceptanceId)
        /// </summary>
        string Signature { get; }

        /// <summary>
        /// Gets the exchange rate used when converting currencies to EUR. Long multiplied by 10000 and rounded to a long value
        /// </summary>
        long ExchangeRate { get; }

        /// <summary>
        /// Gets the additional information about the response
        /// </summary>
        /// <value>The additional information</value>
        /// <remarks>Contains timestamps describing mts processing (receivedUtcTimestamp, validatedUtcTimestamp, respondedUtcTimestamp)</remarks>
        IDictionary<string, string> AdditionalInfo { get; }

        /// <summary>
        /// Gets the automatic accepted odds
        /// </summary>
        /// <value>The automatic accepted odds</value>
        IEnumerable<IAutoAcceptedOdds> AutoAcceptedOdds { get; }
    }
}