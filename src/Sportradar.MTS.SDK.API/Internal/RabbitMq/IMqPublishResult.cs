/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    public interface IMqPublishResult
    {
        bool IsSuccess { get; }

        string Message { get; }

        string CorrelationId { get; }
    }
}
