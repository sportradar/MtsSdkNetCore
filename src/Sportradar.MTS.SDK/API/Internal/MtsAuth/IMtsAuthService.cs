using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sportradar.MTS.SDK.API.Internal.MtsAuth
{
    /// <summary>
    /// Defines contract for implementing MTS authorization service (via Keycloack)
    /// </summary>
    internal interface IMtsAuthService
    {
        /// <summary>
        /// Gets the access token asynchronous.
        /// </summary>
        /// <param name="keycloackUsername">The keycloack username.</param>
        /// <param name="keycloackPassword">The keycloack password.</param>
        /// <returns>Access token if access granted</returns>
        Task<string> GetTokenAsync(string keycloackUsername = null, string keycloackPassword = null);
    }
}
