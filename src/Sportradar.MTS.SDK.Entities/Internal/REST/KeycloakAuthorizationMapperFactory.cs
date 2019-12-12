/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.REST
{
    /// <summary>
    /// Class KeycloakAuthorizationMapperFactory
    /// </summary>
    public class KeycloakAuthorizationMapperFactory : ISingleTypeMapperFactory<AccessTokenDTO, KeycloakAuthorization>
    {
        /// <summary>
        /// Creates and returns an instance of Mapper for mapping <see cref="AccessTokenDTO"/>
        /// </summary>
        /// <param name="data">A input instance which the created <see cref="KeycloakAuthorizationMapper"/> will map</param>
        /// <returns>New <see cref="KeycloakAuthorizationMapper" /> instance</returns>
        public ISingleTypeMapper<KeycloakAuthorization> CreateMapper(AccessTokenDTO data)
        {
            return new KeycloakAuthorizationMapper(data);
        }
    }
}