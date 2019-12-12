/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Interfaces
{
    /// <summary>
    /// Defines a contract for per-selection rejection reasons
    /// </summary>
    public interface ISelectionDetail
    {
        /// <summary>
        /// Gets the index of the selection
        /// </summary>
        int SelectionIndex { get; }

        /// <summary>
        /// Gets the selection response reason
        /// </summary>
        IResponseReason Reason { get; }

        /// <summary>
        /// Gets the rejection information on selection level
        /// </summary>
        /// <value>The rejection information on selection level</value>
        IRejectionInfo RejectionInfo { get; }
    }
}