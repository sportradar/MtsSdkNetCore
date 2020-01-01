/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Internal.Enums;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Defines a contract implemented by classes used to parse the RabbitMq routing key in order to
    /// determine the sportId of the sport associated with the message
    /// </summary>
    internal interface IRoutingKeyParser
    {
        /// <summary>
        /// Gets a string representing the ticketId by parsing the provided <code>routingKey</code>
        /// </summary>
        /// <param name="routingKey">The routing key specified by the feed</param>
        /// <param name="messageTypeName">The type name of the received message</param>
        /// <returns>The sportId obtained by parsing the provided <code>routingKey</code></returns>
        string GetTicketId(string routingKey, string messageTypeName);

        /// <summary>
        /// Tries to get a <see cref="URN"/> representing of the sportId by parsing the provided <code>routingKey</code>
        /// </summary>
        /// <param name="routingKey">The routing key specified by the feed</param>
        /// <param name="messageTypeName">The type name of the received message</param>
        /// <param name="ticketId">If the method returned true the ticketId; Otherwise undefined</param>
        /// <returns>True if ticketId could be retrieved from <code>routingKey</code>; Otherwise false</returns>
        bool TryGetTicketId(string routingKey, string messageTypeName, out string ticketId);
    }
}