/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.REST
{
    /// <summary>
    /// Class CcfMapperFactory
    /// </summary>
    internal class CcfMapperFactory : ISingleTypeMapperFactory<CcfResponseDTO, CcfImpl>
    {
        /// <summary>
        /// Creates and returns an instance of Mapper for mapping <see cref="CcfResponseDTO"/>
        /// </summary>
        /// <param name="data">A input instance which the created <see cref="CcfMapper"/> will map</param>
        /// <returns>New <see cref="CcfMapper" /> instance</returns>
        public ISingleTypeMapper<CcfImpl> CreateMapper(CcfResponseDTO data)
        {
            return new CcfMapper(data);
        }
    }
}