/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Possible values of the odds
    /// </summary>
    public enum OddsChangeType
    {
        /// <summary>
        /// Default behavior
        /// </summary>
        None,

        /// <summary>
        /// Any odds change accepted
        /// </summary>
        Any,

        /// <summary>
        /// Accept higher odds
        /// </summary>
        Higher
    }
}