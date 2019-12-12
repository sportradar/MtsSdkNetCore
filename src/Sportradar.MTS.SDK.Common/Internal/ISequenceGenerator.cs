/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Common.Internal
{
    /// <summary>
    /// Defines a contract implemented by classes used to generate sequence numbers
    /// </summary>
    public interface ISequenceGenerator
    {
        /// <summary>
        /// Gets the next available sequence number
        /// </summary>
        /// <returns>the next available sequence number</returns>
        long GetNext();
    }
}
