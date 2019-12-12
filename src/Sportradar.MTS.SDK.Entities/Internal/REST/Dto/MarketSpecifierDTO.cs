/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;

namespace Sportradar.MTS.SDK.Entities.Internal.REST.Dto
{
    /// <summary>
    /// A data-transfer-object representation for specifier
    /// </summary>
    public class MarketSpecifierDTO
    {
        internal string Name { get; }

        internal string Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketSpecifierDTO"/> class
        /// </summary>
        /// <param name="specifier">The <see cref="desc_specifiersSpecifier"/> used for creating instance</param>
        internal MarketSpecifierDTO(desc_specifiersSpecifier specifier)
        {
            Guard.Argument(specifier).NotNull();
            Guard.Argument(specifier.name).NotNull().NotEmpty();
            Guard.Argument(specifier.type).NotNull().NotEmpty();

            Name = specifier.name;
            Type = specifier.type;
        }
    }
}