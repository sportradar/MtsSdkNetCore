/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using App.Metrics;

namespace Sportradar.MTS.SDK.Common
{
    /// <summary>
    /// Provides methods to get <see cref="IMetricsRoot"/> to record sdk metrics
    /// </summary>
    public static class SdkMetricsFactory
    {
        private static IMetricsRoot _metricsRoot;

        /// <summary>
        /// Set a new instance of the <see cref="SdkMetricsFactory"/> class.
        /// </summary>
        /// <param name="metricsRoot">The metrics root.</param>
        /// <remarks>Used within bootstrapper</remarks>
        internal static void SetMetricsFactory(IMetricsRoot metricsRoot)
        {
            _metricsRoot = metricsRoot;
        }

        /// <summary>
        /// Gets the metrics root
        /// </summary>
        /// <value>The metrics root</value>
        public static IMetricsRoot MetricsRoot
        {
            get
            {
                if (_metricsRoot == null)
                {
                    _metricsRoot = AppMetrics.CreateDefaultBuilder().Build();
                }
                return _metricsRoot;
            }
        }

        /// <summary>
        /// Method for getting <see cref="IMeasureMetrics"/>
        /// </summary>
        /// <returns>Returns <see cref="IMetricsRoot"/> </returns>
        public static IMeasureMetrics GetMeasure()
        {
            return _metricsRoot?.Measure;
        }
    }
}
