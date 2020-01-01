/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Defines possible values for <see cref="IBetReoffer.Type"/>
    /// </summary>
    public enum BetReofferType
    {
        /// <summary>
        /// If auto then stake will be present
        /// </summary>
        Auto,

        /// <summary>
        /// If manual you should wait for reoffer stake over Reply channel
        /// </summary>
        Manual
    }
}