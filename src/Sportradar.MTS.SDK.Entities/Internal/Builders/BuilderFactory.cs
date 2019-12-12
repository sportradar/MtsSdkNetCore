/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Internal.Cache;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// A class used to construct builder instances used when constructing tickets and it's associated entities
    /// </summary>
    /// <seealso cref="Sportradar.MTS.SDK.Entities.Builders.IBuilderFactory" />
    public class BuilderFactory : SimpleBuilderFactory, IBuilderFactory
    {
        /// <summary>
        /// The <see cref="ISdkConfiguration"/> used to set default values of created builders
        /// </summary>
        private readonly ISdkConfigurationInternal _config;

        private readonly IMarketDescriptionProvider _marketDescriptionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuilderFactory"/> class
        /// </summary>
        /// <param name="config">The <see cref="ISdkConfigurationInternal"/> used to set default values of created builders</param>
        /// <param name="marketDescriptionProvider">The <see cref="IMarketDescriptionProvider"/> used for UoF selections</param>
        public BuilderFactory(ISdkConfigurationInternal config, IMarketDescriptionProvider marketDescriptionProvider)
        {
            Guard.Argument(config).NotNull();
            Guard.Argument(marketDescriptionProvider).NotNull();

            _config = config;
            _marketDescriptionProvider = marketDescriptionProvider;
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketCancelBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketCancelBuilder" /> class</returns>
        public ITicketCancelBuilder CreateTicketCancelBuilder()
        {
            return new TicketCancelBuilder(_config);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketReofferCancelBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketReofferCancelBuilder" /> class</returns>
        public ITicketReofferCancelBuilder CreateTicketReofferCancelBuilder()
        {
            return new TicketReofferCancelBuilder(_config);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketCashoutBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketCashoutBuilder" /> class</returns>
        public ITicketCashoutBuilder CreateTicketCashoutBuilder()
        {
            return new TicketCashoutBuilder(_config);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ISenderBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ISenderBuilder" /> class</returns>
        public ISenderBuilder CreateSenderBuilder()
        {
            return new SenderBuilder(_config);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ISelectionBuilder" /> class
        /// </summary>
        /// <param name="isCustomBet">Indicating if selection is for custom bet or not</param>
        /// <returns>A new instance of the <see cref="ISelectionBuilder" /> class</returns>
        public ISelectionBuilder CreateSelectionBuilder(bool isCustomBet = false)
        {
            return new SelectionBuilder(_marketDescriptionProvider, _config, isCustomBet);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketAckBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketAckBuilder" /> class</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ITicketAckBuilder CreateTicketAckBuilder()
        {
            return new TicketAckBuilder(_config);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketCancelAckBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketCancelAckBuilder" /> class</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ITicketCancelAckBuilder CreateTicketCancelAckBuilder()
        {
            return new TicketCancelAckBuilder(_config);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketNonSrSettleBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketNonSrSettleBuilder" /> class</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ITicketNonSrSettleBuilder CreateTicketNonSrSettleBuilder()
        {
            return new TicketNonSrSettleBuilder(_config);
        }
    }
}