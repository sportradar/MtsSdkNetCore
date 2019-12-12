/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;

namespace Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl
{
    /// <summary>
    /// A data-transfer-object for customer confidence factor
    /// </summary>
    public class CcfImpl : ICcf
    {
        public long Ccf { get; }

        public IEnumerable<ISportCcf> SportCcfDetails { get; }

        internal CcfImpl(CcfResponseDTO ccfResponseDto)
        {
            Guard.Argument(ccfResponseDto).NotNull();

            Ccf = ccfResponseDto.Ccf;
            var sportCcfDetails = ccfResponseDto.SportCcfDetails ?? new List<Anonymous>();
            SportCcfDetails = sportCcfDetails.Select(d => new SportCcf(d)).ToList();
        }
    }
}
