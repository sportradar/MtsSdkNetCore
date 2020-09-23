/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Common.Internal.Rest;

// ReSharper disable UnusedMember.Local

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Represents SDK configuration
    /// </summary>
    internal class SdkConfigurationInternal : SdkConfiguration, ISdkConfigurationInternal
    {
        /// <summary>
        /// Default value for the InactivitySeconds property
        /// </summary>
        private const int AmpqDefaultInactivitySeconds = 180;

        /// <summary>
        /// Specifies minimum allowed value of the InactivitySeconds value
        /// </summary>
        private const int AmpqMinInactivitySeconds = 20;

        /// <summary>
        /// The ampq acknowledgment batch limit
        /// </summary>
        private const int AmpqAcknowledgmentBatchLimit = 10;

        /// <summary>
        /// The ampq acknowledgment timeout in seconds
        /// </summary>
        private const int AmpqAcknowledgmentTimeoutInSeconds = 60;

        /// <summary>
        /// Gets the URL of the feed's REST interface
        /// </summary>
        public string ApiHost { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkConfiguration"/> class
        /// </summary>
        public SdkConfigurationInternal(ISdkConfiguration config, IDataFetcher dataFetcher)
            : base(config.Username, config.Password, config.Host, config.VirtualHost, config.UseSsl, config.SslServerName, config.NodeId, config.BookmakerId, config.LimitId, config.Currency, config.Channel, config.AccessToken, config.UfEnvironment, config.ProvideAdditionalMarketSpecifiers, config.Port, config.ExclusiveConsumer)
        {
            Guard.Argument(config, nameof(config)).NotNull();
            ApiHost = null;

            switch (config.UfEnvironment)
            {
                case Entities.Enums.UfEnvironment.Integration:
                    ApiHost = SdkInfo.ApiHostIntegration;
                    return;
                case Entities.Enums.UfEnvironment.Production:
                    ApiHost = SdkInfo.ApiHostProduction;
                    return;
            }

            if (dataFetcher == null)
            {
                ApiHost = SdkInfo.ApiHostIntegration;
            }
            else
            {
                try
                {
                    var result = dataFetcher.GetDataAsync(new Uri($"{SdkInfo.ApiHostProduction}/v1/users/whoami.xml")).Result;
                    ApiHost = SdkInfo.ApiHostProduction;
                    result.Close();
                }
                catch (Exception)
                {
                    ApiHost = SdkInfo.ApiHostIntegration;
                }
            }
        }
    }
}