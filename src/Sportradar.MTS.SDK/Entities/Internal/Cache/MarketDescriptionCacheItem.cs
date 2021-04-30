/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dawn;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Entities.Internal.REST.Dto;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sportradar.MTS.SDK.Entities.Internal.Cache
{
    internal class MarketDescriptionCacheItem
    {
        private static readonly ILogger Log = SdkLoggerFactory.GetLogger(typeof(MarketDescriptionCacheItem));

        private readonly IDictionary<CultureInfo, string> _names;

        private readonly IDictionary<CultureInfo, string> _descriptions;

        private readonly ISet<CultureInfo> _supportedLanguages;

        internal string Variant { get; private set; }

        internal long Id { get; }

        internal IEnumerable<MarketMappingCacheItem> Mappings { get; }

        internal IEnumerable<MarketOutcomeCacheItem> Outcomes { get; }

        internal IEnumerable<MarketSpecifierCacheItem> Specifiers { get; }

        internal IEnumerable<MarketAttributeCacheItem> Attributes { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Allowed")]
        protected MarketDescriptionCacheItem(
            long id,
            IDictionary<CultureInfo, string> names,
            IDictionary<CultureInfo, string> descriptions,
            string variant,
            IEnumerable<MarketOutcomeCacheItem> outcomes,
            IEnumerable<MarketMappingCacheItem> mappings,
            IEnumerable<MarketSpecifierCacheItem> specifiers,
            IEnumerable<MarketAttributeCacheItem> attributes,
            CultureInfo culture)
        {

            Guard.Argument(culture, nameof(culture)).NotNull();
            Guard.Argument(names, nameof(names)).NotNull().NotEmpty();
            Guard.Argument(descriptions, nameof(descriptions)).NotNull();

            Id = id;
            _names = names;
            _descriptions = descriptions;
            _supportedLanguages = new HashSet<CultureInfo>(new[] { culture });
            Outcomes = outcomes;
            Mappings = mappings;
            Specifiers = specifiers;
            Attributes = attributes;
            Variant = variant;
        }

        /// <summary>
        /// Constructs and returns a <see cref="MarketDescriptionCacheItem"/> from the provided DTO
        /// </summary>
        /// <param name="dto">The <see cref="MarketDescriptionDTO"/> containing market description data</param>
        /// <param name="culture">A <see cref="CultureInfo"/> specifying the language of the provided DTO</param>
        /// <returns>The constructed <see cref="MarketDescriptionCacheItem"/></returns>
        /// <exception cref="InvalidOperationException">The cache item could not be build from the provided DTO</exception>
        public static MarketDescriptionCacheItem Build(MarketDescriptionDTO dto, CultureInfo culture)
        {
            Guard.Argument(dto, nameof(dto)).NotNull();
            Guard.Argument(culture, nameof(culture)).NotNull();

            var names = new Dictionary<CultureInfo, string> { { culture, dto.Name } };
            var descriptions = string.IsNullOrEmpty(dto.Description)
                ? new Dictionary<CultureInfo, string>()
                : new Dictionary<CultureInfo, string> { { culture, dto.Description } };

            var outcomes = dto.Outcomes == null
                ? null
                : new ReadOnlyCollection<MarketOutcomeCacheItem>(dto.Outcomes.Select(o => new MarketOutcomeCacheItem(o, culture)).ToList());

            var mappings = dto.Mappings == null
                ? null
                : new ReadOnlyCollection<MarketMappingCacheItem>(dto.Mappings.Select(m => MarketMappingCacheItem.Build(m/*, factory*/)).ToList());

            var specifiers = dto.Specifiers == null
                ? null
                : new ReadOnlyCollection<MarketSpecifierCacheItem>(dto.Specifiers.Select(s => new MarketSpecifierCacheItem(s)).ToList());

            var attributes = dto.Attributes == null
                ? null
                : new ReadOnlyCollection<MarketAttributeCacheItem>(dto.Attributes.Select(a => new MarketAttributeCacheItem(a)).ToList());

            return new MarketDescriptionCacheItem(dto.Id, names, descriptions, dto.Variant, outcomes, mappings, specifiers, attributes, culture);
        }

        internal string GetName(CultureInfo culture)
        {
            Guard.Argument(culture, nameof(culture)).NotNull();

            return _names.TryGetValue(culture, out var name) ? name : null;
        }

        internal string GetDescription(CultureInfo culture)
        {
            Guard.Argument(culture, nameof(culture)).NotNull();

            if (_descriptions.TryGetValue(culture, out var description))
            {
                return description;
            }
            return null;
        }

        public bool HasTranslationsFor(CultureInfo culture)
        {
            return _supportedLanguages.Contains(culture);
        }

        public void Merge(MarketDescriptionDTO dto, CultureInfo culture)
        {
            Guard.Argument(dto, nameof(dto)).NotNull();
            Guard.Argument(culture, nameof(culture)).NotNull();

            _names[culture] = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description))
            {
                _descriptions[culture] = dto.Description;
            }
            Variant = dto.Variant;

            if (dto.Outcomes != null)
            {
                SaveOutcomes(dto.Id, dto.Outcomes.ToList(), culture);
            }

            if (dto.Mappings != null)
            {
                SaveMappings(dto.Id, dto.Mappings.ToList());
            }

            _supportedLanguages.Add(culture);
        }

        private void SaveOutcomes(long marketId, ICollection<OutcomeDescriptionDTO> outcomes, CultureInfo culture)
        {
            foreach (var outcomeDto in outcomes)
            {
                var existingOutcome = Outcomes?.FirstOrDefault(o => o.Id == outcomeDto.Id);
                if (existingOutcome != null)
                {
                    existingOutcome.Merge(outcomeDto, culture);
                }
                else
                {
                    Log.LogWarning($"Could not merge outcome[Id={outcomeDto.Id}] on marketDescription[Id={marketId}] because the specified outcome does not exist on stored market description");
                }
            }
        }

        private void SaveMappings(long marketId, ICollection<MarketMappingDTO> mappings)
        {
            foreach (var mappingDto in mappings)
            {
                var existingMapping = Mappings?.FirstOrDefault(m => m.MarketTypeId == mappingDto.MarketTypeId && m.MarketSubTypeId == mappingDto.MarketSubTypeId);
                if (existingMapping != null)
                {
                    existingMapping.Merge(mappingDto);
                }
                else
                {
                    Log.LogWarning($"Could not merge mapping[MarketId={mappingDto.MarketTypeId}:{mappingDto.MarketSubTypeId}] on marketDescription[Id={marketId}] because the specified mapping" +
                                   " does not exist on stored market description");
                }
            }
        }
    }
}