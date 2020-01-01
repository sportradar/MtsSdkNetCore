/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;

namespace Sportradar.MTS.SDK.Entities.Internal.REST.Dto
{
    /// <summary>
    /// A data-transfer-object representing a market mapping for outcome
    /// </summary>
    internal class OutcomeMappingDTO
    {
        internal string OutcomeId { get; }

        internal string ProductOutcomeId { get; }

        internal string ProductOutcomeName { get; }

        internal OutcomeMappingDTO(mappingsMappingMapping_outcome outcome)
        {
            Guard.Argument(outcome, nameof(outcome)).NotNull();

            OutcomeId = outcome.outcome_id;
            ProductOutcomeId = outcome.product_outcome_id;
            ProductOutcomeName = outcome.product_outcome_name;
        }
    }
}