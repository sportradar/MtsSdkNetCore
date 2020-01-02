/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Net;
using Microsoft.Extensions.Logging;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.EventArguments;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.DemoProject.Example
{
    class NonSrSettle
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger _log;

        private ITicket _originalTicket;

        /// <summary>
        /// The MTS SDK instance
        /// </summary>
        private IMtsSdk _mtsSdk;
        private IBuilderFactory _factory;

        public NonSrSettle(ILogger log)
        {
            _log = log;
        }

        public void Run()
        {
            _log.LogInformation("Running the MTS SDK non-sr settle example");

            _log.LogInformation("Retrieving configuration from application configuration file");
            var config = MtsSdk.GetConfiguration();

            _log.LogInformation("Creating root MTS SDK instance");
            _mtsSdk = new MtsSdk(config);

            _log.LogInformation("Attaching to events");
            AttachToFeedEvents(_mtsSdk);

            _log.LogInformation("Opening the sdk instance (creating and opening connection to the AMPQ broker)");
            _mtsSdk.Open();
            _factory = _mtsSdk.BuilderFactory;

            // create ticket (in order to be accepted, correct values must be entered)
            // values below are just for demonstration purposes and will not be accepted
            var r = new Random();
            _originalTicket = _factory.CreateTicketBuilder()
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
                        .SetStake(123450000, StakeType.Total)
                        .AddSelectedSystem(1)
                        .AddSelection(_factory.CreateSelectionBuilder()
                            .SetEventId("1")
                            .SetId("live:409/1/*/YES")
                            .SetOdds(11000)
                            .Build())
                        .Build())
                .BuildTicket();

            // send ticket to the MTS. Since this is a non-blocking way of sending, the response will raise the event TicketResponseReceived
            _log.LogInformation("Send ticket to the MTS.");
            _mtsSdk.SendTicket(_originalTicket);

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

            _log.LogInformation("Attaching to events");
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

            _log.LogInformation("Detaching from events");
            mtsSdk.SendTicketFailed -= OnSendTicketFailed;
            mtsSdk.TicketResponseReceived -= OnTicketResponseReceived;
            mtsSdk.UnparsableTicketResponseReceived -= OnUnparsableTicketResponseReceived;
        }

        private void OnTicketResponseReceived(object sender, TicketResponseReceivedEventArgs e)
        {
            _log.LogInformation($"Received {e.Type}Response for ticket '{e.Response.TicketId}'.");

            if (e.Type == TicketResponseType.Ticket)
            {
                HandleTicketResponse((ITicketResponse)e.Response);
            }
            else if (e.Type == TicketResponseType.TicketCancel)
            {
                HandleTicketCancelResponse((ITicketCancelResponse)e.Response);
            }
            else if (e.Type == TicketResponseType.TicketCashout)
            {
                HandleTicketCashoutResponse((ITicketCashoutResponse)e.Response);
            }
        }

        private void OnUnparsableTicketResponseReceived(object sender, UnparsableMessageEventArgs e)
        {
            _log.LogInformation($"Received unparsable ticket response: {e.Body}.");
        }

        private void OnSendTicketFailed(object sender, TicketSendFailedEventArgs e)
        {
            _log.LogInformation($"Sending ticket '{e.TicketId}' failed.");
        }

        private void HandleTicketResponse(ITicketResponse ticketResponse)
        {
            _log.LogInformation($"Ticket '{ticketResponse.TicketId}' response is {ticketResponse.Status}. Reason={ticketResponse.Reason?.Message}");
            if (ticketResponse.Status == TicketAcceptance.Accepted)
            {
                //required only if 'explicit acking' is enabled in MTS admin
                ticketResponse.Acknowledge();

                // handle ticket response

                //if for some reason we want to cashout ticket, this is how we can do it
                var ticketCashout = _factory.CreateTicketCashoutBuilder().SetTicketId(ticketResponse.TicketId).SetCashoutStake(12932).BuildTicket();
                _mtsSdk.SendTicket(ticketCashout);
            }
        }

        private void HandleTicketCancelResponse(ITicketCancelResponse ticket)
        {
            _log.LogInformation($"Ticket '{ticket.TicketId}' response is {ticket.Status}. Reason={ticket.Reason?.Message}");
            if (ticket.Status == TicketCancelAcceptance.Cancelled)
            {
                //required only if 'explicit acking' is enabled in MTS admin
                ticket.Acknowledge();
            }
        }

        private void HandleTicketCashoutResponse(ITicketCashoutResponse ticketCashoutResponse)
        {
            _log.LogInformation($"Ticket '{ticketCashoutResponse.TicketId}' response is {ticketCashoutResponse.Status}. Reason={ticketCashoutResponse.Reason?.Message}");
            if (ticketCashoutResponse.Status == CashoutAcceptance.Accepted)
            {
                ticketCashoutResponse.Acknowledge();
            }
        }
    }
}
