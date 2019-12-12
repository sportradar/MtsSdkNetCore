/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;

namespace Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl
{
    /// <summary>
    /// A data-transfer-object for Keycloak authorization
    /// </summary>
    public class KeycloakAuthorization
    {
        public string AccessToken { get; }

        public DateTimeOffset Expires { get; }

        internal KeycloakAuthorization(AccessTokenDTO authorization)
        {
            Guard.Argument(authorization).NotNull();
            Guard.Argument(authorization.Access_token).NotNull();
            Guard.Argument(authorization.Expires_in).Positive();

            AccessToken = authorization.Access_token;
            Expires = DateTimeOffset.Now.AddSeconds(authorization.Expires_in);
        }
    }
}
