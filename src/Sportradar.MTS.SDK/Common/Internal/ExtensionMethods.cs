/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using System.IO;
using System.Text;

namespace Sportradar.MTS.SDK.Common.Internal
{
    /// <summary>
    /// Defines extension methods used by the SDK
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Gets a <see cref="string"/> representation of the provided <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> whose content to get</param>
        /// <returns>A <see cref="string"/> representation of the <see cref="Stream"/> content</returns>
        public static string GetData(this Stream stream)
        {
            Guard.Argument(stream, nameof(stream)).NotNull();

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
