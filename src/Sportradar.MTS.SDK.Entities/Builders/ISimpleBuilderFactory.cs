/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
namespace Sportradar.MTS.SDK.Entities.Builders
{
    /// <summary>
    /// Defines a contract used to construct simple builder instances(those not needing configuration) used when constructing tickets and it's associated entities
    /// </summary>

    public interface ISimpleBuilderFactory
    {
        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketBuilder"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketBuilder"/> class</returns>
        ITicketBuilder CreateTicketBuilder();

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketReofferBuilder"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketReofferBuilder"/> class</returns>
        ITicketReofferBuilder CreateTicketReofferBuilder();

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="IEndCustomerBuilder"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="IEndCustomerBuilder"/> class</returns>
        IEndCustomerBuilder CreateEndCustomerBuilder();

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="IBetBuilder"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="IBetBuilder"/> class</returns>
        IBetBuilder CreateBetBuilder();

        /// <summary>
        /// Constructs and returns a new instance of the <see cref="ITicketAltStakeBuilder"/> class
        /// </summary>
        /// <returns>A new instance of the <see cref="ITicketAltStakeBuilder"/> class</returns>
        ITicketAltStakeBuilder CreateAltStakeBuilder();
    }
}