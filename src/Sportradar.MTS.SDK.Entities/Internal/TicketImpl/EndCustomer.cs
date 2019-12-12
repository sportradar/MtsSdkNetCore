/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    public class EndCustomer : IEndCustomer
    {
        public string Ip { get; }
        public string LanguageId { get; }
        public string DeviceId { get; }

        /// <summary>
        /// Gets the end user's unique id (in client's system)
        /// </summary>
        public string Id { get; }

        public long Confidence { get; }

        [JsonConstructor]
        public EndCustomer(string ip, string languageId, string deviceId, string id, long confidence)
        {
            Guard.Argument(languageId).Require(string.IsNullOrEmpty(languageId) || languageId.Length == 2 || languageId.Length == 3);
            Guard.Argument(deviceId).Require(string.IsNullOrEmpty(deviceId) || TicketHelper.ValidateUserId(deviceId));
            Guard.Argument(id).Require(string.IsNullOrEmpty(id) || TicketHelper.ValidateUserId(id));
            Guard.Argument(confidence).NotNegative();

            if (!string.IsNullOrEmpty(ip))
            {
                IPAddress.Parse(ip);
                Ip = ip;
            }
            LanguageId = languageId;
            DeviceId = deviceId;
            Id = id;
            Confidence = confidence;
        }

        public EndCustomer(IPAddress ip, string languageId, string deviceId, string id, long confidence)
        {
            Guard.Argument(languageId).Require(string.IsNullOrEmpty(languageId) || languageId.Length == 2);
            Guard.Argument(deviceId).Require(string.IsNullOrEmpty(deviceId) || TicketHelper.ValidateUserId(deviceId));
            Guard.Argument(id).Require(string.IsNullOrEmpty(id) || TicketHelper.ValidateUserId(id));
            Guard.Argument(confidence).NotNegative();

            Ip = ip?.ToString();
            LanguageId = languageId;
            DeviceId = deviceId;
            Id = id;
            Confidence = confidence;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("Ip", Ip);
            info.AddValue("LanguageId", LanguageId);
            info.AddValue("DeviceId", DeviceId);
            info.AddValue("Id", Id);
            info.AddValue("Confidence", Confidence);
        }
    }
}