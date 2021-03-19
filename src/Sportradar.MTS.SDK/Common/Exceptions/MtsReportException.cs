/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Runtime.Serialization;
using Sportradar.MTS.SDK.API;

namespace Sportradar.MTS.SDK.Common.Exceptions
{
    /// <summary>
    /// An exception thrown by the SDK when there is a problem with communication via <see cref="IReportManager"/>
    /// </summary>
    /// <seealso cref="FeedSdkException" />
    [Serializable]
    public class MtsReportException : FeedSdkException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MtsReportException"/> class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        public MtsReportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MtsReportException"/> class
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination</param>
        protected MtsReportException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
