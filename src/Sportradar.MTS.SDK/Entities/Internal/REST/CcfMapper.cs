/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Internal.Dto.ClientApi;
using Sportradar.MTS.SDK.Entities.Internal.REST.ClientApiImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.REST
{
    internal class CcfMapper : ISingleTypeMapper<CcfImpl>
    {
        /// <summary>
        /// A <see cref="CcfResponseDTO"/> instance containing data used to construct <see cref="CcfImpl"/> instance
        /// </summary>
        private readonly CcfResponseDTO _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="CcfMapper"/> class
        /// </summary>
        /// <param name="data">A <see cref="CcfResponseDTO"/> instance containing data used to construct <see cref="CcfImpl"/> instance</param>
        internal CcfMapper(CcfResponseDTO data)
        {
            Guard.Argument(data, nameof(data)).NotNull();

            _data = data;
        }

        /// <summary>
        /// Maps it's data to <see cref="CcfImpl"/> instance
        /// </summary>
        /// <returns>The created <see cref="CcfImpl"/> instance</returns>
        CcfImpl ISingleTypeMapper<CcfImpl>.Map()
        {
            return new CcfImpl(_data);
        }
    }
}