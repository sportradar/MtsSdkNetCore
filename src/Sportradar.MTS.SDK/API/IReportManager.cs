using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sportradar.MTS.SDK.Common.Exceptions;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.API
{
    /// <summary>
    /// Defines methods used to get various reports
    /// </summary>
    public interface IReportManager
    {
        /// <summary>
        /// Gets the customer ccf (confidence factor history change) CSV output stream for bookmaker with filters
        /// </summary>
        /// <param name="outputStream">The output stream to store the result data (in CSV format)</param>
        /// <param name="startDate">The start date to query changes</param>
        /// <param name="endDate">The end date to query changes</param>
        /// <param name="bookmakerId">The bookmaker id - (if not set via configuration)</param>
        /// <param name="subBookmakerIds">The sub bookmaker ids</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="username">A username used for authentication - (if not set via configuration - keycloakUsername)</param>
        /// <param name="password">A password used for authentication - (if not set via configuration - keycloakPassword)</param>
        /// <exception cref="MtsReportException">If there is exception getting report data</exception>
        Task GetHistoryCcfChangeCsvExportAsync(Stream outputStream,
                                                DateTime startDate,
                                                DateTime endDate,
                                                int? bookmakerId = null,
                                                List<int> subBookmakerIds = null,
                                                string sourceId = null,
                                                SourceType sourceType = SourceType.Customer,
                                                string username = null,
                                                string password = null);


        /// <summary>
        /// Gets the customer ccf (confidence factor history change) as list of <see cref="ICcfChange"/> for bookmaker with filters
        /// </summary>
        /// <param name="startDate">The start date to query changes</param>
        /// <param name="endDate">The end date to query changes</param>
        /// <param name="bookmakerId">The bookmaker id - (if not set via configuration)</param>
        /// <param name="subBookmakerIds">The sub bookmaker ids</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="username">A username used for authentication - (if not set via configuration - keycloakUsername)</param>
        /// <param name="password">A password used for authentication - (if not set via configuration - keycloakPassword)</param>
        /// <returns>Returns the list of <see cref="ICcfChange"/></returns>
        /// <exception cref="MtsReportException">If there is exception getting report data</exception>
        Task<List<ICcfChange>> GetHistoryCcfChangeCsvExportAsync(DateTime startDate, 
                                                                DateTime endDate, 
                                                                int? bookmakerId = null, 
                                                                List<int> subBookmakerIds = null, 
                                                                string sourceId = null, 
                                                                SourceType sourceType = SourceType.Customer, 
                                                                string username = null, 
                                                                string password = null);
    }
}
