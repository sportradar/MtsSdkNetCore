using System;
using System.Collections.Generic;
using System.Text;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Sportradar.MTS.SDK.Entities.Internal.REST.ReportApiImpl
{
    /// <summary>
    /// Class CcfChange.
    /// Implements the <see cref="ICcfChange" />
    /// </summary>
    /// <seealso cref="ICcfChange" />
    internal class CcfChange : ICcfChange
    {
        /// <summary>
        /// Gets the timestamp of the ccf value change
        /// </summary>
        /// <value>The timestamp of the ccf value change</value>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets the bookmaker id of the ccf value change
        /// </summary>
        /// <value>The bookmaker id of the ccf value change</value>
        public int BookmakerId { get; set; }

        /// <summary>
        /// Gets the sub bookmaker id of the ccf value change
        /// </summary>
        /// <value>The sub bookmaker id of the ccf value change</value>
        public int SubBookmakerId { get; set; }

        /// <summary>
        /// Gets the source id of the ccf value change
        /// </summary>
        /// <value>The source id of the ccf value change</value>
        public string SourceId { get; set; }

        /// <summary>
        /// Gets the source type customer of the ccf value change
        /// </summary>
        /// <value>The source type customer of the ccf value change</value>
        public SourceType SourceType { get; set; }

        /// <summary>
        /// Gets the customer confidence factor for the customer
        /// </summary>
        /// <value>The customer confidence factor for the customer</value>
        public double Ccf { get; set; }

        /// <summary>
        /// Gets the previous customer confidence factor for the customer
        /// </summary>
        /// <value>The previous customer confidence factor for the customer</value>
        public double? PreviousCcf { get; set; }

        /// <summary>
        /// Gets the sport id
        /// </summary>
        /// <value>The sport id</value>
        public string SportId { get; set; }

        /// <summary>
        /// Gets the name of the sport
        /// </summary>
        /// <value>The name of the sport</value>
        public string SportName { get; set; }

        /// <summary>
        /// Gets a value indicating whether the change was for live only
        /// </summary>
        /// <value><c>null</c> if [is live] contains no value, <c>true</c> if [is live]; otherwise, <c>false</c>.</value>
        public bool? IsLive { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Timestamp={Timestamp}, BookmakerId={BookmakerId}, SubBookmakerId={SubBookmakerId}, SourceId={SourceId}, SourceType={SourceType}, Ccf={Ccf}, PreviousCcf={PreviousCcf}, SportId={SportId}, SportName={SportName}, IsLive={IsLive}";
        }
    }
}
