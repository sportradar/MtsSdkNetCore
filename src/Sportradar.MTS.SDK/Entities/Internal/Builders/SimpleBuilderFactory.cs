/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Builders;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// A class used to construct simple builder instances(those not needing configuration) used when constructing tickets and it's associated entities
    /// </summary>
    internal class SimpleBuilderFactory : ISimpleBuilderFactory
    {
        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketBuilder" /> class</returns>
        public ITicketBuilder CreateTicketBuilder()
        {
            return new TicketBuilder();
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketReofferBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketReofferBuilder" /> class</returns>
        public ITicketReofferBuilder CreateTicketReofferBuilder()
        {
            return new TicketReofferBuilder(this);
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="IEndCustomerBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="IEndCustomerBuilder" /> class</returns>
        public IEndCustomerBuilder CreateEndCustomerBuilder()
        {
            return new EndCustomerBuilder();
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="IBetBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="IBetBuilder" /> class</returns>
        public IBetBuilder CreateBetBuilder()
        {
            return new BetBuilder();
        }

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketAltStakeBuilder" /> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketAltStakeBuilder" /> class</returns>
        public ITicketAltStakeBuilder CreateAltStakeBuilder()
        {
            return new TicketAltStakeBuilder(this);
        }
    }
}