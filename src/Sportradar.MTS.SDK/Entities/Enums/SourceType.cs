using System;
using System.Collections.Generic;
using System.Text;

namespace Sportradar.MTS.SDK.Entities.Enums
{
    /// <summary>
    /// Possible values used for getting Ccf history change report
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// Shop
        /// </summary>
        Shop,

        /// <summary>
        /// Terminal
        /// </summary>
        Terminal,
        
        /// <summary>
        /// Customer
        /// </summary>
        Customer,

        /// <summary>
        /// Bookmaker
        /// </summary>
        Bookmaker,

        /// <summary>
        /// SubBookmaker
        /// </summary>
        SubBookmaker,

        /// <summary>
        /// Distribution channel
        /// </summary>
        DistributionChannel
    }
}
