/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using Dawn;
using System.IO;
using System.Xml;
using Sportradar.MTS.SDK.Common.Exceptions;
using Sportradar.MTS.SDK.Common.Internal;
using Sportradar.MTS.SDK.Entities.Internal.REST;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    /// <summary>
    /// A <see cref="IDeserializer{T}" /> implementation which uses <see cref="XmlElement.LocalName" /> property to determine to
    /// which type the data should be deserialized
    /// </summary>
    /// <typeparam name="T">Specifies the type that can be deserialized</typeparam>
    internal class Deserializer<T> : DeserializerBase, IDeserializer<T> where T : class
    {
        /// <summary>
        /// Deserialize the provided<see cref="byte"/> array to a <see cref="RestMessage"/> derived instance
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> instance containing data to be deserialized </param>
        /// <returns>The <code>data</code> deserialized to <see cref="RestMessage"/> instance</returns>
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

            using var reader = new NamespaceIgnorantXmlTextReader(stream);
            bool startElementFound;
            try
            {
                startElementFound = reader.IsStartElement();
            }
            catch (XmlException ex)
            {
                throw new DeserializationException("The format of the XML is not correct", stream.GetData(), null, ex);
            }

            if (!startElementFound)
            {
                throw new DeserializationException("Could not retrieve the name of the root element", stream.GetData(), null, null);
            }

            var localName = reader.LocalName;
            if (!Serializers.TryGetValue(reader.LocalName, out var serializerWithInfo))
            {
                throw new DeserializationException("Specified root element is not supported", stream.GetData(), localName, null);
            }
            reader.IgnoreNamespace = serializerWithInfo.IgnoreNamespace;

            try
            {
                return (T1)serializerWithInfo.Serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ex)
            {
                throw new DeserializationException("Deserialization failed", stream.GetData(), localName, ex.InnerException ?? ex);
            }
        }

        public T1 Deserialize<T1>(string input) where T1 : T
        {
            Guard.Argument(input, nameof(input)).NotNull().NotEmpty();

            throw new NotImplementedException();
        }

        /// <summary>
        /// A <see cref="XmlReader"/> derived class, which is capable of deserializing Odds Feed REST messages. Those messages have schema issues
        /// which this class handles. Once the schema issues are fixed this class will be removed
        /// </summary>
        private class NamespaceIgnorantXmlTextReader : XmlTextReader
        {
            public bool IgnoreNamespace { private get; set; }
            
            public override string NamespaceURI { get; }

            public NamespaceIgnorantXmlTextReader(Stream stream)
                : base(stream)
            {
                NamespaceURI = string.Empty;
                if (IgnoreNamespace)
                {
                    NamespaceURI = string.IsNullOrWhiteSpace(base.NamespaceURI) ? string.Empty : SdkInfo.DefaultNamespaceUri;
                }
                else
                {
                    NamespaceURI = base.NamespaceURI;
                }
            }
        }
    }
}