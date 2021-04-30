/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */

using Sportradar.MTS.SDK.Common.Exceptions;

namespace Sportradar.MTS.SDK.Entities.Internal.REST
{
    /// <summary>
    /// Defines a contract implemented by classes capable of map data to instance of type specified by out parameter
    /// </summary>
    /// <typeparam name="T">Specifies the target type of the <see cref="ISingleTypeMapper{T}"/></typeparam>
    internal interface ISingleTypeMapper<out T> where T : class
    {
        /// <summary>
        /// Maps it's data to instance of <typeparamref name="T"/>
        /// </summary>
        /// <returns>The created <typeparamref name="T"/> instance</returns>
        /// <exception cref="MappingException">The mapping of the entity failed</exception>
        T Map();
    }
}