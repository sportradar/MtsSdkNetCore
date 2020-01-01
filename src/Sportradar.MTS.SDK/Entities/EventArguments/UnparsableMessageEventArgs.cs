/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;

namespace Sportradar.MTS.SDK.Entities.EventArguments
{
    /// <summary>
    /// Event arguments of UnparsableMessageReceived event
    /// </summary>
    public class UnparsableMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="string"/> representation of the JSON body associated with the unparsable message
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Body { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnparsableMessageEventArgs"/> class
        /// </summary>
        /// <param name="json">The string within the unparsable message</param>
        public UnparsableMessageEventArgs(string json)
        {
            Body = json;
        }
    }
}