/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.IO;

namespace Sportradar.MTS.SDK.Common.Internal
{
    /// <summary>
    /// Defines a contract implemented by classes used to deserialize feed messages to
    /// <typeparam name="T">Defines the base that can be deserialized using the <see cref="IDeserializer{T}"/></typeparam>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3246:Generic type parameters should be co/contravariant when possible", Justification = "Cant be in this case")]
    internal interface IDeserializer<T> where T : class
    {
        /// <summary>
        /// Deserialize the provided <see cref="byte"/> array to instance of T
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance containing data to be deserialized </param>
        /// <returns>The <code>data</code> deserialized to instance of T</returns>
        /// <exception cref="Exceptions.DeserializationException">The deserialization failed</exception>
        T Deserialize(Stream stream);

        /// <summary>
        /// Deserialize the provided <see cref="byte"/> array to instance of T
        /// </summary>
        /// <typeparam name="T1">Specifies the type to which to deserialize the data</typeparam>
        /// <param name="stream">A <see cref="Stream"/> instance containing data to be deserialized </param>
        /// <returns>The <code>data</code> deserialized to instance of T1</returns>
        /// <exception cref="Exceptions.DeserializationException">The deserialization failed</exception>
        T1 Deserialize<T1>(Stream stream) where T1 : T;

        /// <summary>
        /// Deserialize the provided <see cref="byte"/> array to instance of T
        /// </summary>
        /// <typeparam name="T1">Specifies the type to which to deserialize the data</typeparam>
        /// <param name="input">A (JSON) string text containing data to be deserialized</param>
        /// <returns>The <code>data</code> deserialized to instance of T1</returns>
        /// <exception cref="Exceptions.DeserializationException">The deserialization failed</exception>
        T1 Deserialize<T1>(string input) where T1 : T;
    }
}