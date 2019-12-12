/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.IO;
using System.Reflection;
using System.Xml;
using Sportradar.MTS.SDK.Entities;
using Sportradar.MTS.SDK.Entities.Internal;

namespace Sportradar.MTS.SDK.Test.Helpers
{
    public static class SdkConfigurationSectionHelper
    {
        public static ISdkConfiguration ToSdkConfiguration(this string xmlConfig)
        {
            using (var stringReader = new StringReader(xmlConfig))
            using (var xmlReader = XmlReader.Create(stringReader))
                return xmlReader.ToSdkConfiguration();
        }

        private static ISdkConfiguration ToSdkConfiguration(this XmlReader reader)
        {
            var config = new SdkConfigurationSection();
            var deserializer = config.GetType().GetMethod("DeserializeSection",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, new[] {typeof(XmlReader)},
                null);
            deserializer?.Invoke(config, new object[] {reader});
            return new SdkConfiguration(config);
        }
    }
}
