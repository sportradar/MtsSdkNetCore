/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.API.Internal.RabbitMq
{
    /// <summary>
    /// Represents a factory used to construct Rabbit MQ channels / models
    /// </summary>
    public interface IChannelFactory
    {
        /// <summary>
        /// Gets the unique id channel can use
        /// </summary>
        /// <returns>The unique id</returns>
        int GetUniqueId();

        /// <summary>
        /// Returns a <see cref="ChannelWrapper"/> containing a channel used to communicate with the broker
        /// </summary>
        /// <param name="id">Unique id of the channel</param>
        /// <returns>a <see cref="ChannelWrapper"/> containing a channel used to communicate with the broker</returns>
        ChannelWrapper GetChannel(int id);

        /// <summary>
        /// Removes the channel
        /// </summary>
        /// <param name="id">The identifier</param>
        void RemoveChannel(int id);
    }
}
