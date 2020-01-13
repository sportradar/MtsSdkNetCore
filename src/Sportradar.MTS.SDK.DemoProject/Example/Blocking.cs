/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.EventArguments;

namespace Sportradar.MTS.SDK.DemoProject.Example
{
    /// <summary>
    /// Basic example for creating and sending ticket
    /// </summary>
    public class Blocking
    {
        private IMtsSdk _mtsSdk;

        private IBuilderFactory _factory;

        private readonly ILogger _log;
        private readonly ILoggerFactory _loggerFactory;

        public Blocking(ILoggerFactory loggerFactory = null)
        {
            _loggerFactory = loggerFactory;
            _log = _loggerFactory?.CreateLogger(typeof(Basic)) ?? new NullLogger<Basic>();
        }

        public void Run()
        {
            _log.LogInformation("Running the MTS SDK Basic example");

            _log.LogInformation("Retrieving configuration from application configuration file");
            var config = MtsSdk.GetConfiguration();

            _log.LogInformation("Creating root MTS SDK instance");
            _mtsSdk = new MtsSdk(config, _loggerFactory);

            _log.LogInformation("Attaching to events");
            AttachToFeedEvents(_mtsSdk);

            _log.LogInformation("Opening the sdk instance (creating and opening connection to the AMPQ broker)");
            _mtsSdk.Open();
            _factory = _mtsSdk.BuilderFactory;

            // create ticket (in order to be accepted, correct values must be entered)
            // values below are just for demonstration purposes and will not be accepted
            var r = new Random();
            var ticket = _factory.CreateTicketBuilder()
                .SetTicketId("ticketId-" + r.Next())
                .SetSender(_factory.CreateSenderBuilder()
                    .SetCurrency("EUR")
                    .SetEndCustomer(_factory.CreateEndCustomerBuilder()
                        .SetId("customerClientId-" + r.Next())
                        .SetConfidence(1)
                        .SetIp(IPAddress.Loopback)
                        .SetLanguageId("en")
                        .SetDeviceId("UsersDevice-" + r.Next())
                        .Build())
                    .Build())
                .AddBet(_factory.CreateBetBuilder()
                        .SetBetId("betId-" + r.Next())
                        .SetBetBonus(1)
                        .SetStake(15000, StakeType.Total)
                        .AddSelectedSystem(1)
                        .AddSelection(_factory.CreateSelectionBuilder()
                            .SetEventId("1")
                            .SetId("lcoo:409/1/*/YES")
                            .SetOdds(11000)
                            .Build())
                        .Build())
                .BuildTicket();

            // send ticket to the MTS. Since this is a blocking way of sending, the response will be result of the method (no event handler will be raised)
            _log.LogInformation("Send ticket to the MTS and wait for the response.");
            var ticketResponse = _mtsSdk.SendTicketBlocking(ticket);
            _log.LogInformation($"TicketResponse received. Status={ticketResponse.Status}, Reason={ticketResponse.Reason.Message}.");

            if (ticketResponse.Status == TicketAcceptance.Accepted)
            {
                //required only if 'explicit acking' is enabled in MTS admin
                ticketResponse.Acknowledge();

                //if for some reason we want to cancel ticket, this is how we can do it
                var ticketCancel = _factory.CreateTicketCancelBuilder().BuildTicket(ticket.TicketId, ticket.Sender.BookmakerId, TicketCancellationReason.BookmakerTechnicalIssue);
                var ticketCancelResponse = _mtsSdk.SendTicketCancelBlocking(ticketCancel);

                _log.LogInformation($"Ticket '{ticket.TicketId}' response is {ticketCancelResponse.Status}. Reason={ticketCancelResponse.Reason?.Message}");
                if (ticketCancelResponse.Status == TicketCancelAcceptance.Cancelled)
                {
                    //required only if 'explicit acking' is enabled in MTS admin
                    ticketCancelResponse.Acknowledge();
                }
            }

            _log.LogInformation("Example successfully executed. Hit <enter> to quit");
            Console.WriteLine(string.Empty);
            Console.ReadLine();

            _log.LogInformation("Detaching from events");
            DetachFromFeedEvents(_mtsSdk);

            _log.LogInformation("Closing the connection and disposing the instance");
            _mtsSdk.Close();


            _log.LogInformation("Example stopped");
        }

        /// <summary>
        /// Attaches to events raised by <see cref="IMtsSdk"/>
        /// </summary>
        /// <param name="mtsSdk">A <see cref="IMtsSdk"/> instance </param>
        private void AttachToFeedEvents(IMtsSdk mtsSdk)
        {
            if (mtsSdk == null)
            {
                throw new ArgumentNullException(nameof(mtsSdk));
            }

            mtsSdk.SendTicketFailed += OnSendTicketFailed;
            mtsSdk.TicketResponseReceived += OnTicketResponseReceived;
            mtsSdk.UnparsableTicketResponseReceived += OnUnparsableTicketResponseReceived;
        }

        /// <summary>
        /// Detaches from events defined by <see cref="IMtsSdk"/>
        /// </summary>
        /// <param name="mtsSdk">A <see cref="IMtsSdk"/> instance</param>
        private void DetachFromFeedEvents(IMtsSdk mtsSdk)
        {
            if (mtsSdk == null)
            {
                throw new ArgumentNullException(nameof(mtsSdk));
            }

            mtsSdk.SendTicketFailed -= OnSendTicketFailed;
            mtsSdk.TicketResponseReceived -= OnTicketResponseReceived;
            mtsSdk.UnparsableTicketResponseReceived -= OnUnparsableTicketResponseReceived;
        }

        private void OnTicketResponseReceived(object sender, TicketResponseReceivedEventArgs e)
        {
            //in the blocking scenario this should never be raised
            _log.LogInformation($"Received {e.Type}Response for ticket '{e.Response.TicketId}'.");
        }

        private void OnUnparsableTicketResponseReceived(object sender, UnparsableMessageEventArgs e)
        {
            _log.LogInformation($"Received unparsable ticket response: {e.Body}.");
        }

        private void OnSendTicketFailed(object sender, TicketSendFailedEventArgs e)
        {
            _log.LogInformation($"Sending ticket '{e.TicketId}' failed.");
        }
    }
}
