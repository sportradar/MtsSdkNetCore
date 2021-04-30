/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Collections.Generic;
using Dawn;
using System.Linq;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// Defines builder for routing keys and checks for feed sessions combo validation
    /// </summary>
    internal static class SdkRoutingKeyBuilder
    {
        /// <summary>
        /// Validates input list of message interests and returns list of routing keys combination per interest
        /// </summary>
        /// <param name="interests"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<string>> GenerateKeys(IEnumerable<MessageInterest> interests)
        {
            Guard.Argument(interests, nameof(interests)).NotNull().NotEmpty();

            var sessionKeys = new List<List<string>>();
            var messageInterests = interests.ToList();

            // only 1 interest allowed
            var interest = messageInterests.First();
            sessionKeys.Add(GetBaseKeys(interest, 1).ToList());

            return sessionKeys;
        }

        /// <summary>
        /// Gets the standard keys usually added to all sessions
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;</returns>
        public static IEnumerable<string> GetStandardKeys()
        {
            return new List<string>
            {
                "-.-.-.-.#",
                "-.-.-.-.#"
            };
        }

        /// <summary>
        /// Gets the system keys added to system session
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;</returns>
        public static IEnumerable<string> GetSystemKeys()
        {
            return new List<string>
            {
                "-.-.-.pd.#",
                "-.-.-.ue.#"
            };
        }

        private static IEnumerable<string> GetBaseKeys(MessageInterest interest, int nodeId)
        {
            if (interest == MessageInterest.TicketSubmit)
            {
                return TicketSubmitMessages(nodeId);
            }
            if (interest == MessageInterest.TicketCancel)
            {
                return TicketCancelMessages(nodeId);
            }
            throw new ArgumentOutOfRangeException(nameof(interest), "Unknown MessageInterest.");
        }

        /// <summary>
        /// Constructs a <see cref="MessageInterest"/> indicating an interest in ticket submit messages
        /// </summary>
        /// <returns>A <see cref="MessageInterest"/> indicating an interest in ticket submit messages</returns>
        private static IEnumerable<string> TicketSubmitMessages(int nodeId)
        {
            if (nodeId < 1)
            {
                nodeId = 1;
            }
            return new[]
            {
                $"node{nodeId}.ticket.confirm"
            };
        }

        /// <summary>
        /// Constructs a <see cref="MessageInterest"/> indicating an interest in ticket cancel messages
        /// </summary>
        /// <returns>A <see cref="MessageInterest"/> indicating an interest in ticket cancel messages</returns>
        private static IEnumerable<string> TicketCancelMessages(int nodeId)
        {
            if (nodeId < 1)
            {
                nodeId = 1;
            }
            return new[]
            {
                $"node{nodeId}.cancel.confirm"
            };
        }
    }
}