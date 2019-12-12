/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Defines a contract implemented by classes representing odds feed configuration / settings
    /// </summary>

    public interface ISdkConfigurationInternal : ISdkConfiguration
    {
        /// <summary>
        /// Gets the URL of the feed's REST interface
        /// </summary>
        string ApiHost { get; }
    }
}