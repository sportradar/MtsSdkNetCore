/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.IO;
using Dawn;
using Newtonsoft.Json;
using Sportradar.MTS.SDK.Common.Internal;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// A <see cref="IDeserializer{T}" /> implementation for json responses
    /// </summary>
    /// <typeparam name="T">Specifies the type that can be deserialized</typeparam>
    internal class JsonDeserializer<T> : IDeserializer<T> where T : class
    {
        /// <summary>
        /// Deserialize the provided<see cref="byte"/> array
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance containing data to be deserialized </param>
        /// <returns>The <code>data</code> deserialized to <typeparamref name="T" /> instance</returns>
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
        public T1 Deserialize<T1>(Stream stream) where T1 : T
        {
            Guard.Argument(stream, nameof(stream)).NotNull();

            using (var reader = new StreamReader(stream))
            {
                return Deserialize<T1>(reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Deserialize the provided <see cref="byte" /> array to a <typeparamref name="T1" /> instance
        /// </summary>
        /// <typeparam name="T1">A <typeparamref name="T" /> derived type specifying the target of deserialization</typeparam>
        /// <param name="input">A <see cref="string" /> containing data to be deserialized</param>
        /// <returns>The <code>data</code> deserialized to <typeparamref name="T1" /> instance</returns>
        public T1 Deserialize<T1>(string input) where T1 : T
        {
            Guard.Argument(input, nameof(input)).NotNull().NotEmpty();

            return JsonConvert.DeserializeObject<T1>(input);
        }
    }
}