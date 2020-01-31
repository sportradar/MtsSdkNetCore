/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    internal enum ExchangeType
    {
        Direct = 0,

        Fanout = 1,

        Topic = 2
    }
}
