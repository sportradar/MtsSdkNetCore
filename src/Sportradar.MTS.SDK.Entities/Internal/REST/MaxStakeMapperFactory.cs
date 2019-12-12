/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.REST
{
    /// <summary>
    /// Class MarketDescriptionsMapperFactory
    /// </summary>
    public class MaxStakeMapperFactory : ISingleTypeMapperFactory<MaxStakeResponseDTO, MaxStakeImpl>
    {
        /// <summary>
        /// Creates and returns an instance of Mapper for mapping <see cref="MaxStakeResponseDTO"/>
        /// </summary>
        /// <param name="data">A input instance which the created <see cref="MaxStakeMapper"/> will map</param>
        /// <returns>New <see cref="MaxStakeMapper" /> instance</returns>
        public ISingleTypeMapper<MaxStakeImpl> CreateMapper(MaxStakeResponseDTO data)
        {
            return new MaxStakeMapper(data);
        }
    }
}