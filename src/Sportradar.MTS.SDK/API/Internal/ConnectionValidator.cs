/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Internal;

namespace Sportradar.MTS.SDK.API.Internal
{
    /// <summary>
    /// Used to validate connection to the message broker
    /// </summary>
    internal class ConnectionValidator
    {
        /// <summary>
        /// A <see cref="ISdkConfiguration"/> instance representing odds configuration
        /// </summary>
        private readonly ISdkConfigurationInternal _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionValidator"/> class
        /// </summary>
        /// <param name="config">A <see cref="ISdkConfiguration"/> instance representing odds configuration</param>
        public ConnectionValidator(ISdkConfigurationInternal config)
        {
            Guard.Argument(config, nameof(config)).NotNull();

            _config = config;
        }

        /// <summary>
        /// Validates connection to the message broker
        /// </summary>
        /// <returns>A <see cref="ConnectionValidationResult"/> enum member specifying the result of validation</returns>
        public ConnectionValidationResult ValidateConnection()
        {
            using var client = new TcpClient();
            try
            {
                var host = _config.Host.Replace(_config.UseSsl ? "http://" : "https://", string.Empty);
                client.Connect(host, _config.Port);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10060 || ex.ErrorCode == 10061)
                {
                    return ConnectionValidationResult.ConnectionRefused;
                }
                return ex.ErrorCode >= 11001 && ex.ErrorCode <= 11004
                    ? ConnectionValidationResult.NoInternetConnection
                    : ConnectionValidationResult.Unknown;
            }
            return ConnectionValidationResult.Success;
        }

        /// <summary>
        /// Gets the public IP of the current machine
        /// </summary>
        /// <returns>A <see cref="IPAddress"/> representing the IP of the current machine or a null reference or a null reference if public IP could not be determined</returns>
        public IPAddress GetPublicIp()
        {
            string data;

            try
            {
                var client = new HttpClient();
                var stream = client.GetStreamAsync(new Uri(SdkInfo.PublicIpDomain)).Result;
                using var reader = new StreamReader(stream, Encoding.UTF8);
                data = reader.ReadToEnd();
            }
            catch (AggregateException)
            {
                return null;
            }

            return IPAddress.TryParse(data, out var address)
                ? address
                : null;
        }
    }
}
