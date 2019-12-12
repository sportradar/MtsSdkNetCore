/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.REST
{
    internal class MaxStakeMapper : ISingleTypeMapper<MaxStakeImpl>
    {
        /// <summary>
        /// A <see cref="MaxStakeResponseDTO"/> instance containing data used to construct <see cref="MaxStakeImpl"/> instance
        /// </summary>
        private readonly MaxStakeResponseDTO _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxStakeMapper"/> class
        /// </summary>
        /// <param name="data">A <see cref="MaxStakeResponseDTO"/> instance containing data used to construct <see cref="MaxStakeImpl"/> instance</param>
        internal MaxStakeMapper(MaxStakeResponseDTO data)
        {
            Guard.Argument(data).NotNull();

            _data = data;
        }

        /// <summary>
        /// Maps it's data to <see cref="MaxStakeImpl"/> instance
        /// </summary>
        /// <returns>The created <see cref="MaxStakeImpl"/> instance</returns>
        MaxStakeImpl ISingleTypeMapper<MaxStakeImpl>.Map()
        {
            return new MaxStakeImpl(_data);
        }
    }
}