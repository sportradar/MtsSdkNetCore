/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Sportradar.MTS.SDK.API.Internal;
using Sportradar.MTS.SDK.Common.Internal.Rest;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Cache;
using Unity;
using Unity.Lifetime;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sportradar.MTS.SDK.Test.Helpers
{
    internal class BuilderFactoryHelper
    {
        public static readonly IEnumerable<Tuple<string, string>> UriReplacements = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("/v1/descriptions/en/markets.xml?include_mappings=true", "market_descriptions.en.xml")
        };

        public BuilderFactoryHelper(ISdkConfigurationInternal configInternal = null)
        {
            if (configInternal == null)
            {
                configInternal = new SdkConfigurationInternal(new SdkConfiguration(SdkConfigurationSectionTest.Create()), null);
            }

            var container = new UnityContainer();
            var config = (ISdkConfiguration) configInternal;
            container.RegisterTypes(config, null, null);

            container.RegisterInstance<IDataFetcher>("Base", new DataFetcherHelper(UriReplacements), new ContainerControlledLifetimeManager());

            MarketDescriptionCache = container.Resolve<IMarketDescriptionCache>();

            MarketDescriptionProvider = container.Resolve<IMarketDescriptionProvider>();

            BuilderFactory = container.Resolve<IBuilderFactory>();
        }

        public IBuilderFactory BuilderFactory { get; }

        public IMarketDescriptionCache MarketDescriptionCache { get; }

        public IMarketDescriptionProvider MarketDescriptionProvider { get; }
    }
}