﻿/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Internal.Dto.CustomBet;

namespace Sportradar.MTS.SDK.Entities.Internal.REST
{
    /// <summary>
    /// A factory used to create <see cref="ISingleTypeMapper{T}"/> instances used to map <see cref="AvailableSelectionsType"/> instances to
    /// <see cref="AvailableSelectionsDTO"/> instances
    /// </summary>
    /// <seealso cref="ISingleTypeMapperFactory{TOut,TIn}" />
    internal class AvailableSelectionsMapperFactory : ISingleTypeMapperFactory<AvailableSelectionsType, AvailableSelectionsDTO>
    {
        /// <summary>
        /// Creates and returns a <see cref="ISingleTypeMapper{T}" /> instance used to map <see cref="AvailableSelectionsType"/> instances to
        /// <see cref="AvailableSelectionsDTO"/> instances
        /// </summary>
        /// <param name="data">A <see cref="AvailableSelectionsType" /> instance which the created <see cref="ISingleTypeMapper{T}" /> will map</param>
        /// <returns>The constructed <see cref="ISingleTypeMapper{T}" /> instance</returns>
        public ISingleTypeMapper<AvailableSelectionsDTO> CreateMapper(AvailableSelectionsType data)
        {
            return new AvailableSelectionsMapper(data);
        }
    }
}
