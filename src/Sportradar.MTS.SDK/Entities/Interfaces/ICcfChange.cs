/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Sportradar.MTS.SDK.Entities.Enums;

namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for objects containing CCF change data
    /// </summary>
    public interface ICcfChange
    {
        /// <summary>
        /// Gets the timestamp of the ccf value change
        /// </summary>
        /// <value>The timestamp of the ccf value change</value>
        DateTime Timestamp { get; }
        
        /// <summary>
        /// Gets the bookmaker id of the ccf value change
        /// </summary>
        /// <value>The bookmaker id of the ccf value change</value>
        int BookmakerId { get; }
        
        /// <summary>
        /// Gets the sub bookmaker id of the ccf value change
        /// </summary>
        /// <value>The sub bookmaker id of the ccf value change</value>
        int SubBookmakerId { get; }

        /// <summary>
        /// Gets the source id of the ccf value change
        /// </summary>
        /// <value>The source id of the ccf value change</value>
        string SourceId { get; }

        /// <summary>
        /// Gets the source type customer of the ccf value change
        /// </summary>
        /// <value>The source type customer of the ccf value change</value>
        SourceType SourceType { get; }

        /// <summary>
        /// Gets the customer confidence factor for the customer
        /// </summary>
        /// <value>The customer confidence factor for the customer</value>
        double Ccf { get; }

        /// <summary>
        /// Gets the previous customer confidence factor for the customer
        /// </summary>
        /// <value>The previous customer confidence factor for the customer</value>
        double? PreviousCcf { get; }

        /// <summary>
        /// Gets the sport id
        /// </summary>
        /// <value>The sport id</value>
        string SportId { get; }

        /// <summary>
        /// Gets the name of the sport
        /// </summary>
        /// <value>The name of the sport</value>
        string SportName { get; }

        /// <summary>
        /// Gets a value indicating whether the change was for live only
        /// </summary>
        /// <value><c>null</c> if [is live] contains no value, <c>true</c> if [is live]; otherwise, <c>false</c>.</value>
        bool? IsLive { get; }
    }
}
