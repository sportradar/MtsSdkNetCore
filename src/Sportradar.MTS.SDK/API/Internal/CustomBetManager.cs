/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Common.Exceptions;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces.CustomBet;
using Sportradar.MTS.SDK.Entities.Internal;
using Sportradar.MTS.SDK.Entities.Internal.Builders;
using Sportradar.MTS.SDK.Entities.Internal.CustomBetImpl;
using Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet;

namespace Sportradar.MTS.SDK.API.Internal
{
    /// <summary>
    /// The run-time implementation of the <see cref="ICustomBetManager"/> interface
    /// </summary>
    internal class CustomBetManager : ICustomBetManager
    {
        private readonly ILogger _clientLog = SdkLoggerFactory.GetLoggerForClientInteraction(typeof(CustomBetManager));
        private readonly ILogger _executionLog = SdkLoggerFactory.GetLoggerForExecution(typeof(CustomBetManager));

        private readonly IDataProvider<AvailableSelectionsDTO> _availableSelectionsProvider;
        private readonly ICalculateProbabilityProvider _calculateProbabilityProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomBetManager"/> class
        /// </summary>
        /// <param name="availableSelectionsProvider">A <see cref="IDataProvider{AvailableSelectionsDTO}"/> used to make custom bet API requests</param>
        /// <param name="calculateProbabilityProvider">A <see cref="ICalculateProbabilityProvider"/> used to make custom bet API requests</param>
        public CustomBetManager(IDataProvider<AvailableSelectionsDTO> availableSelectionsProvider, ICalculateProbabilityProvider calculateProbabilityProvider)
        {
            if (availableSelectionsProvider == null)
            {
                throw new ArgumentNullException(nameof(availableSelectionsProvider));
            }

            if (calculateProbabilityProvider == null)
            {
                throw new ArgumentNullException(nameof(calculateProbabilityProvider));
            }

            _availableSelectionsProvider = availableSelectionsProvider;
            _calculateProbabilityProvider = calculateProbabilityProvider;
        }

        public async Task<IAvailableSelections> GetAvailableSelectionsAsync(string eventId)
        {
            if (eventId == null)
            {
                throw new ArgumentNullException(nameof(eventId));
            }

            try
            {
                _clientLog.LogInformation($"Invoking CustomBetManager.GetAvailableSelectionsAsync({eventId})");
                var availableSelections = await _availableSelectionsProvider.GetDataAsync(eventId).ConfigureAwait(false);
                return new AvailableSelections(availableSelections);
            }
            catch (CommunicationException ce)
            {
                _executionLog.LogWarning($"Event[{eventId}] getting available selections failed, CommunicationException: {ce.Message}");
                throw;
            }
            catch (Exception e)
            {
                _executionLog.LogWarning($"Event[{eventId}] getting available selections failed.", e);
                throw;
            }
        }

        public async Task<ICalculation> CalculateProbabilityAsync(IEnumerable<ISelection> selections)
        {
            if (selections == null)
            {
                throw new ArgumentNullException(nameof(selections));
            }

            try
            {
                _clientLog.LogInformation($"Invoking CustomBetManager.CalculateProbability({selections})");
                var calculationResponse = await _calculateProbabilityProvider.GetDataAsync(selections).ConfigureAwait(false);
                return new Calculation(calculationResponse);
            }
            catch (CommunicationException ce)
            {
                _executionLog.LogWarning($"Calculating probabilities failed, CommunicationException: {ce.Message}");
                throw;
            }
            catch (Exception e)
            {
                _executionLog.LogWarning($"Calculating probabilities failed.", e);
                throw;
            }
        }

        public ICustomBetSelectionBuilder CustomBetSelectionBuilder => new CustomBetSelectionBuilder();
    }
}
