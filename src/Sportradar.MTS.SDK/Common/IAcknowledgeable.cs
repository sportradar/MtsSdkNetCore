/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Common
{
    /// <summary>
    /// Defines a contract for classes used to provide acknowledgment
    /// </summary>
    public interface IAcknowledgeable
    {
        /// <summary>
        /// Send acknowledgment back to MTS
        /// </summary>
        /// <param name="markAccepted">if set to <c>true</c> [mark accepted]</param>
        void Acknowledge(bool markAccepted = true);

        /// <summary>
        /// Send acknowledgment back to MTS
        /// </summary>
        /// <param name="markAccepted">if set to <c>true</c> [mark accepted]</param>
        /// <param name="bookmakerId">The sender identifier (bookmakerId)</param>
        /// <param name="code">The code</param>
        /// <param name="message">The message</param>
        void Acknowledge(bool markAccepted, int bookmakerId, int code, string message);
    }
}
