/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Dawn;
using Sportradar.MTS.SDK.Common.Exceptions;
using Sportradar.MTS.SDK.Common.Internal;

namespace Sportradar.MTS.SDK.API.Internal
{
    /// <summary>
    /// A <see cref="IDeserializer{T}" /> implementation which uses <see cref="DataContractJsonSerializer" /> property to deserialize JSON strings
    /// </summary>
    /// <typeparam name="T">Specifies the type that can be deserialized</typeparam>
    public class JsonDeserializer<T> : IDeserializer<T> where T : class
    {
        /// <summary>
        /// Deserialize the provided <see cref="byte"/> array to T derived instance
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance containing data to be deserialized </param>
        /// <returns>The <code>data</code> deserialized to instance of T</returns>
        /// <exception cref="DeserializationException">The deserialization failed</exception>
        public T Deserialize(Stream stream)
        {
            Guard.Argument(stream, nameof(stream)).NotNull();

            return Deserialize<T>(stream);
        }

        /// <summary>
        /// Deserialize the provided <see cref="byte" /> array to a <typeparamref name="T1" /> instance
        /// </summary>
        /// <typeparam name="T1">A <typeparamref name="T" /> derived type specifying the target of deserialization</typeparam>
        /// <param name="stream">A <see cref="Stream" /> instance containing data to be deserialized</param>
        /// <returns>The <code>data</code> deserialized to <typeparamref name="T1" /> instance</returns>
        /// <exception cref="DeserializationException">The deserialization failed</exception>
        public T1 Deserialize<T1>(Stream stream) where T1 : T
        {
            Guard.Argument(stream, nameof(stream)).NotNull();

            var obj = Activator.CreateInstance<T1>();
            var serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T1)serializer.ReadObject(stream);
            stream.Close();
            return obj;
        }

        /// <summary>
        /// Deserialize the specified json string
        /// </summary>
        /// <typeparam name="T1">The type to be converted to</typeparam>
        /// <param name="input">The json containing data to be deserialized</param>
        /// <returns>T1</returns>
        public T1 Deserialize<T1>(string input) where T1 : T
        {
            Guard.Argument(input, nameof(input)).NotNull().NotEmpty();

            var ms = new MemoryStream(Encoding.Unicode.GetBytes(input));
            return Deserialize<T1>(ms);
        }
    }
}
