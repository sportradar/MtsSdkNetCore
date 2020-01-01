/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Defines which messages will be provided by feed
    /// </summary>
    internal class MessageInterest
    {
        /// <summary>
        /// Constructs a <see cref="MessageInterest"/> indicating an interest in ticket submit messages
        /// </summary>
        /// <returns>A <see cref="MessageInterest"/> indicating an interest in ticket submit messages</returns>
        public static readonly MessageInterest TicketSubmit = new MessageInterest();

        /// <summary>
        /// Constructs a <see cref="MessageInterest"/> indicating an interest in ticket cancel messages
        /// </summary>
        /// <returns>A <see cref="MessageInterest"/> indicating an interest in ticket cancel messages</returns>
        public static readonly MessageInterest TicketCancel = new MessageInterest();
    }
}