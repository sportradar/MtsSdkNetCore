using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Dawn;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.Common;
using Sportradar.MTS.SDK.Entities.Internal.REST;

namespace Sportradar.MTS.SDK.Entities.Internal
{
    internal class DeserializerBase
    {
        private static readonly ILogger Logger = SdkLoggerFactory.GetLogger(typeof(DeserializerBase));

        /// <summary>
        /// A list of <see cref="Type"/> specifying base types which are supported by the deserializer. All subclasses
        /// of the specified types can be deserialized by the deserializer
        /// </summary>
        private static readonly Type[] BaseTypes = { typeof(XmlRestMessage) };

        /// <summary>
        /// A <see cref="IReadOnlyDictionary{String, XmlSerializer}"/> containing serializers for all supported types
        /// </summary>
        protected static readonly IDictionary<string, SerializerWithInfo> Serializers = InitDeserializer();

        /// <summary>
        /// Initializes the <code>Serializers</code> static field
        /// </summary>
        private static Dictionary<string, SerializerWithInfo> InitDeserializer()
        {
            var serializers = new Dictionary<string, SerializerWithInfo>();

            foreach (var baseType in BaseTypes)
            {
                foreach (var feedMessagesType in baseType.Assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t)))
                {
                    var xmlRootAttribute = feedMessagesType.GetCustomAttribute<XmlRootAttribute>(false);
                    var ignoreNamespaceAttribute = feedMessagesType.GetCustomAttribute<OverrideXmlNamespaceAttribute>(false);

                    var rootElementName = xmlRootAttribute == null || string.IsNullOrWhiteSpace(xmlRootAttribute.ElementName)
                        ? ignoreNamespaceAttribute?.RootElementName
                        : xmlRootAttribute.ElementName;

                    if (string.IsNullOrWhiteSpace(rootElementName))
                    {
                        Logger.LogError($"Type {feedMessagesType.FullName} cannot be deserialized with {typeof(Deserializer<>).FullName} because the name of RootXmlElement is not specified");
                        continue;
                    }
                    if (serializers.ContainsKey(rootElementName))
                    {
                        Logger.LogWarning($"Deserializer associated with name {rootElementName} already exists");
                        continue;
                    }

                    var ignoreNamespace = ignoreNamespaceAttribute?.IgnoreNamespace ?? false;

                    serializers.Add(rootElementName, new SerializerWithInfo(new XmlSerializer(feedMessagesType), ignoreNamespace));
                }
            }

            return serializers;
        }

        internal class SerializerWithInfo
        {
            public XmlSerializer Serializer { get; }

            public bool IgnoreNamespace { get; }

            public SerializerWithInfo(XmlSerializer serializer, bool ignoreNamespace)
            {
                Guard.Argument(serializer, nameof(serializer)).NotNull();

                Serializer = serializer;
                IgnoreNamespace = ignoreNamespace;
            }
        }
    }
}