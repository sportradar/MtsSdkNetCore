/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Entities.EventArguments;

namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    /// <summary>
    /// Defines a contract for AMPQ message receiver
    /// </summary>
    /// <seealso cref="Sportradar.MTS.SDK.Common.IOpenable" />
    internal interface IRabbitMqMessageReceiver : IOpenable
    {
        /// <summary>
        /// Opens the current channel and binds the created queue to provided routing keys
        /// </summary>
        /// <param name="routingKeys">A <see cref="IEnumerable{String}"/> specifying the routing keys of the constructed queue</param>
        void Open(IEnumerable<string> routingKeys);

        /// <summary>
        /// Opens the current channel and binds the created queue to provided routing keys
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="routingKeys">A <see cref="IEnumerable{String}"/> specifying the routing keys of the constructed queue</param>
        void Open(string queueName, IEnumerable<string> routingKeys);

        /// <summary>
        /// Occurs when the current channel received the data
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MqMessageReceived;
    }
}
