/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sportradar.MTS.SDK.Common
{
    /// <summary>
    /// Provides methods to get logger for specific <see cref="LoggerType"/>
    /// </summary>
    public static class SdkLoggerFactory
    {
        /// <summary>
        /// Default repository name for the SDK
        /// </summary>
        internal const string SdkLogRepositoryName = "Sportradar.MTS.SDK";

        private static ILoggerFactory _factory;
        
        /// <summary>
        /// Set a new instance of the <see cref="SdkLoggerFactory"/> class
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <remarks>Used within bootstrapper</remarks>
        internal static void SetLoggerFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Gets the logger factory
        /// </summary>
        /// <value>The logger factory</value>
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new NullLoggerFactory();
                }
                return _factory;
            }
        }

        /// <summary>
        /// Method for getting log4net.ILog for feed traffic
        /// </summary>
        /// <param name="type">A type to be used for creating new ILog</param>
        /// <param name="repositoryName">Repository containing the logger</param>
        /// <returns>Returns default <see cref="ILogger"/> with specified settings</returns>
        public static ILogger GetLoggerForFeedTraffic(Type type, string repositoryName = SdkLogRepositoryName)
        {
            return GetLogger(type, repositoryName, LoggerType.FeedTraffic);
        }

        /// <summary>
        /// Method for getting log4net.ILog for rest traffic
        /// </summary>
        /// <param name="type">A type to be used for creating new ILog</param>
        /// <param name="repositoryName">Repository containing the logger</param>
        /// <returns>Returns default <see cref="ILogger"/> with specified settings</returns>
        public static ILogger GetLoggerForRestTraffic(Type type, string repositoryName = SdkLogRepositoryName)
        {
            return GetLogger(type, repositoryName, LoggerType.RestTraffic);
        }

        /// <summary>
        /// Method for getting log4net.ILog for client iteration
        /// </summary>
        /// <param name="type">A type to be used for creating new ILog</param>
        /// <param name="repositoryName">Repository containing the logger</param>
        /// <returns>Returns default <see cref="ILogger"/> with specified settings</returns>
        public static ILogger GetLoggerForClientInteraction(Type type, string repositoryName = SdkLogRepositoryName)
        {
            return GetLogger(type, repositoryName, LoggerType.ClientInteraction);
        }

        /// <summary>
        /// Method for getting log4net.ILog for cache
        /// </summary>
        /// <param name="type">A type to be used for creating new ILog</param>
        /// <param name="repositoryName">Repository containing the logger</param>
        /// <returns>Returns default <see cref="ILogger"/> with specified settings</returns>
        public static ILogger GetLoggerForCache(Type type, string repositoryName = SdkLogRepositoryName)
        {
            return GetLogger(type, repositoryName, LoggerType.Cache);
        }

        /// <summary>
        /// Method for getting log4net.ILog for statistics
        /// </summary>
        /// <param name="type">A type to be used for creating new ILog</param>
        /// <param name="repositoryName">Repository containing the logger</param>
        /// <returns>Returns default <see cref="ILogger"/> with specified settings</returns>
        public static ILogger GetLoggerForStats(Type type, string repositoryName = SdkLogRepositoryName)
        {
            return GetLogger(type, repositoryName, LoggerType.Stats);
        }

        /// <summary>
        /// Method for getting log4net.ILog for execution
        /// </summary>
        /// <param name="type">A type to be used for creating new ILog</param>
        /// <param name="repositoryName">Repository containing the logger</param>
        /// <returns>Returns default <see cref="ILogger"/> with specified settings</returns>
        public static ILogger GetLoggerForExecution(Type type, string repositoryName = SdkLogRepositoryName)
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            return GetLogger(type, repositoryName, LoggerType.Execution);
        }

        /// <summary>
        /// Method for getting correct <see cref="ILogger"/> for specific logger type
        /// </summary>
        /// <param name="type">A type to be used for creating new ILog</param>
        /// <param name="repositoryName">Repository containing the logger</param>
        /// <param name="loggerType">A value of <see cref="LoggerType"/> to be used to get <see cref="ILogger"/></param>
        /// <returns>Returns <see cref="ILogger"/> with specific settings for this <see cref="LoggerType"/></returns>
        public static ILogger GetLogger(Type type, string repositoryName = SdkLogRepositoryName, LoggerType loggerType = LoggerType.Execution)
        {
            if (loggerType == LoggerType.Execution)
            {
                return LoggerFactory.CreateLogger(type);
            }
            var key = SdkLogRepositoryName + "." + Enum.GetName(typeof(LoggerType), loggerType);
            return LoggerFactory.CreateLogger(key);
        }
    }
}