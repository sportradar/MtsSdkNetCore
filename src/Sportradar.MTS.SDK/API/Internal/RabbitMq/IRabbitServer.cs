/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;

namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    /// <summary>
    /// Represents a contract implemented by classes used to connect to rabbit mq broker
    /// </summary>
    internal interface IRabbitServer
    {
        /// <summary>
        /// Gets the username used to connect to server
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the password used to connect to server
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets a value indicating whether to use SSL to connect to server
        /// </summary>
        bool UseSsl { get; }

        /// <summary>
        /// Gets the server name that will be used to check against SSL certificate
        /// </summary>
        string SslServerName { get; }

        /// <summary>
        /// Gets the virtual host on the connected server
        /// </summary>
        string VirtualHost { get; }

        /// <summary>
        /// Gets the port used to connect to server
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Gets the address used to connect to server
        /// </summary>
        string HostAddress { get; }

        /// <summary>
        /// Gets a value indicating whether automatic recovery should be set or not
        /// </summary>
        bool AutomaticRecovery { get; }

        /// <summary>
        /// Gets the heart beat that should be used on connection (in seconds)
        /// </summary>
        /// <value>0 means use default value (default:60)</value>
        ushort HeartBeat { get; }

        /// <summary>
        /// Gets the client properties to be added to connection
        /// </summary>
        IDictionary<string, object> ClientProperties { get; }
    }
}
