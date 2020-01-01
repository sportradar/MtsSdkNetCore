/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    internal class MqPublishResult : IMqPublishResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string CorrelationId { get; }

        internal MqPublishResult(string correlationId, bool success = true, string message = null)
        {
            CorrelationId = correlationId;
            IsSuccess = success;
            Message = message ?? string.Empty;
        }
    }
}
