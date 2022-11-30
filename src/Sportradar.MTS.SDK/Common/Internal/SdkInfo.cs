/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Linq;
using System.Reflection;

namespace Sportradar.MTS.SDK.Common.Internal
{
    /// <summary>
    /// Class provides information about current executing assembly
    /// </summary>
    internal static class SdkInfo
    {
        public const int TicketResponseTimeoutLiveDefault = 17000;
        public const int TicketResponseTimeoutPrematchDefault = 5000;
        public const int TicketCancellationResponseTimeoutDefault = 600000;
        public const int TicketCashoutResponseTimeoutDefault = 600000;
        public const int TicketNonSrResponseTimeoutDefault = 600000;
        public const int TicketResponseTimeoutLiveMin = 10000;
        public const int TicketResponseTimeoutPrematchMin = 3000;
        public const int TicketCancellationResponseTimeoutMin = 10000;
        public const int TicketCashoutResponseTimeoutMin = 10000;
        public const int TicketNonSrResponseTimeoutMin = 10000;
        public const int TicketResponseTimeoutLiveMax = 30000;
        public const int TicketResponseTimeoutPrematchMax = 30000;
        public const int TicketCancellationResponseTimeoutMax = 3600000;
        public const int TicketCashoutResponseTimeoutMax = 3600000;
        public const int TicketNonSrResponseTimeoutMax = 3600000;
        public const string ApiHostIntegration = "https://global.stgapi.betradar.com";
        public const string ApiHostProduction = "https://global.api.betradar.com";
        public const string PublicIpDomain = "http://ipecho.net/plain";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "Approved")]
        public const string DefaultNamespaceUri = "http://schemas.sportradar.com/sportsapi/v1/unified";

        /// <summary>
        /// Gets the version number of the executing assembly
        /// </summary>
        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Gets the assembly version number
        /// </summary>
        public static string GetVersion(Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }

        /// <summary>
        /// Multiplies the specified value
        /// </summary>
        /// <param name="value">The initial value</param>
        /// <param name="factor">The factor</param>
        /// <param name="maxValue">The maximum value</param>
        /// <returns>The multiplied value, up to max value</returns>
        public static int Multiply(int value, double factor = 2, int maxValue = 64000)
        {
            value = (int) (value * factor);
            if (value >= maxValue)
            {
                value = maxValue;
            }
            return value;
        }


        /// <summary>
        /// Increase the specified value
        /// </summary>
        /// <param name="value">The initial value</param>
        /// <param name="factor">The factor (if 0 is *2)</param>
        /// <param name="maxValue">The maximum value</param>
        /// <returns>The increased value, up to max value</returns>
        public static int Increase(int value, int factor = 0, int maxValue = 64000)
        {
            if (factor == 0)
            {
                value *= 2;
            }
            else
            {
                value += factor;
            }
            if (value >= maxValue)
            {
                value = maxValue;
            }
            return value;
        }

        public static string Obfuscate(string input, bool checkDash = false)
        {
            if(string.IsNullOrEmpty(input))
            {
                return input;
            }

            if(checkDash)
            {
                var inputSplit = input.Split('-');
                if (input.Split('-').Length > 1)
                {
                    return $"{inputSplit.First()}-***-{inputSplit.Last()}";
                }
            }

            if (input.Length > 6)
            {
                var first3 = input.Substring(0, 3);
                var last3 = input.Substring(input.Length - 3);
                return $"{first3}***{last3}";
            }

            if (input.Length > 2)
            {
                return $"{input.First()}***{input.Last()}";
            }

            return "***";
        }
    }
}
