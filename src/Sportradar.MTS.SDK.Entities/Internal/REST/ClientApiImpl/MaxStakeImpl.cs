/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;

namespace Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl
{
    /// <summary>
    /// A data-transfer-object for max stake
    /// </summary>
    public class MaxStakeImpl
    {
        public long MaxStake { get; }

        internal MaxStakeImpl(MaxStakeResponseDTO maxStakeResponseDto)
        {
            Guard.Argument(maxStakeResponseDto, nameof(maxStakeResponseDto)).NotNull();

            MaxStake = maxStakeResponseDto.MaxStake;
        }
    }
}
