/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for object carrying information about rejection cause
    /// </summary>
    public interface IResponseReason
    {
        /// <summary>
        /// Gets the reason code
        /// </summary>
        int Code { get; }

        /// <summary>
        /// Gets the reason message
        /// </summary>
        string Message { get; }
    }
}