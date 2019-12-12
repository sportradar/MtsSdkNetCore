/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Internal.Enums
{
    /// <summary>
    /// Enumerates groups of resources represented by the <see cref="URN"/>
    /// </summary>
    public enum ResourceTypeGroup
    {
        /// <summary>
        /// The resource represents a sport event of match type
        /// </summary>
        Match,

        /// <summary>
        /// The resource represents a sport event of race type
        /// </summary>
        Race,

        /// <summary>
        /// The resource represents a tournament
        /// </summary>
        Tournament,

        /// <summary>
        /// The non-specific URN type specifier
        /// </summary>
        Other
    }
}