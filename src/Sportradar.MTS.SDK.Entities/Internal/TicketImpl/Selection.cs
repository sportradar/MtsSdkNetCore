/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Class Selection
    /// </summary>
    /// <seealso cref="ISelection" />
    public class Selection : ISelection
    {
        /// <summary>
        /// Gets the Betradar event (match or outright) id
        /// </summary>
        /// <value>The event identifier</value>
        public string EventId { get; }
        /// <summary>
        /// Gets the selection id
        /// </summary>
        /// <value>Should be composed according to specification</value>
        public string Id { get; }
        /// <summary>
        /// Gets the odds multiplied by 10000 and rounded to int value
        /// </summary>
        /// <value>The odds</value>
        public int? Odds { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is banker
        /// </summary>
        /// <value><c>true</c> if this instance is banker; otherwise, <c>false</c></value>
        public bool IsBanker { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Selection"/> class
        /// </summary>
        /// <param name="eventId">The event identifier</param>
        /// <param name="id">The identifier</param>
        /// <param name="odds">The odds</param>
        /// <param name="isBanker">if set to <c>true</c> [is banker]</param>
        [JsonConstructor]
        public Selection(string eventId, string id, int? odds, bool isBanker = false)
        {
            Guard.Argument(eventId).NotNull().NotEmpty();
            Guard.Argument(eventId.Length).InRange(1, 50);
            Guard.Argument(id).NotNull().NotEmpty();
            Guard.Argument(id.Length).InRange(1, 1000);
            Guard.Argument(odds).Require(odds == null || (odds >= 10000 && odds <= 1000000000));


            EventId = eventId;
            Id = id;
            Odds = odds;
            IsBanker = isBanker;
        }

        public override bool Equals(object obj)
        {
            var sel = (Selection) obj;
            return sel != null && sel.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return $"{EventId}+{Id}+{Odds}+{IsBanker}".GetHashCode();
        }
    }
}