/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;

namespace Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl
{
    /// <summary>
    /// A data-transfer-object for customer confidence factor per sport
    /// </summary>
    internal class SportCcf : ISportCcf
    {
        public string SportId { get; }

        public long PrematchCcf { get; }

        public long LiveCcf { get; }

        internal SportCcf(Anonymous sportCcf)
        {
            Guard.Argument(sportCcf, nameof(sportCcf)).NotNull();
            Guard.Argument(sportCcf.SportId, nameof(sportCcf.SportId)).NotNull();

            SportId = sportCcf.SportId;
            PrematchCcf = sportCcf.PrematchCcf;
            LiveCcf = sportCcf.LiveCcf;
        }
    }
}