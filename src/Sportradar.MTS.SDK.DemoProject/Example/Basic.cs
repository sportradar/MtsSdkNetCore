/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.EventArguments;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.DemoProject.Example
{
    /// <summary>
    /// Basic example for creating and sending ticket
    /// </summary>
    public class Basic
    {
        private IMtsSdk _mtsSdk;

        private IBuilderFactory _factory;

        private readonly ILogger _log;
        private readonly ILoggerFactory _loggerFactory;

        public Basic(ILoggerFactory loggerFactory = null)
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
                        .SetStake(11200, StakeType.Total)
                        .AddSelectedSystem(1)
                        .AddSelection(_factory.CreateSelectionBuilder()
                            .SetEventId("1")
                            //.SetIdUof(3, $"sr:match:{r.Next()}", 12, "1", string.Empty, null) // only one of the following is required to set selectionId (if you use this method, config property 'accessToken' must be provided)
                            .SetIdLo(1, 1, "1:1", "1")
                            .SetIdLcoo(1, 1, "1:1", "1")
                            .SetId("lcoo:409/1/*/YES")
                            .SetOdds(11000)
                            .Build())
                        .Build())
                .BuildTicket();

            // send ticket to the MTS. Since this is a non-blocking way of sending, the response will come on the event TicketResponseReceived
            _log.LogInformation("Send ticket to the MTS.");
            _log.LogInformation(ticket.ToJson());
            _mtsSdk.SendTicket(ticket);

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
            mtsSdk.TicketResponseTimedOut += OnTicketResponseTimedOut;
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
            mtsSdk.TicketResponseTimedOut -= OnTicketResponseTimedOut;
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
        }

        private void OnUnparsableTicketResponseReceived(object sender, UnparsableMessageEventArgs e)
        {
            _log.LogInformation($"Received unparsable ticket response: {e.Body}.");
        }

        private void OnSendTicketFailed(object sender, TicketSendFailedEventArgs e)
        {
            _log.LogInformation($"Sending ticket '{e.TicketId}' failed.");
        }
        private void OnTicketResponseTimedOut(object sender, TicketMessageEventArgs e)
        {
            _log.LogInformation($"Sending ticket '{e.TicketId}' failed due to timeout.");
        }

        private void HandleTicketResponse(ITicketResponse ticket)
        {
            _log.LogInformation($"Ticket '{ticket.TicketId}' response is {ticket.Status}. Reason={ticket.Reason?.Message}");

            if (ticket.BetDetails != null && ticket.BetDetails.Any())
            {
                foreach (var betDetail in ticket.BetDetails)
                {
                    _log.LogInformation($"Bet decline reason: '{betDetail.Reason?.Message}'.");
                    if (betDetail.SelectionDetails != null && betDetail.SelectionDetails.Any())
                    {
                        foreach (var selectionDetail in betDetail.SelectionDetails)
                        {
                            _log.LogInformation($"Selection decline reason: '{selectionDetail.Reason?.Message}'.");
                        }
                    }
                }
            }

            if (ticket.Status == TicketAcceptance.Accepted)
            {
                //required only if 'explicit acking' is enabled in MTS admin
                ticket.Acknowledge();

                // handle ticket response

                //if for some reason we want to cancel ticket, this is how we can do it
                var ticketCancel = _factory.CreateTicketCancelBuilder().SetTicketId(ticket.TicketId).SetCode(TicketCancellationReason.BookmakerTechnicalIssue).BuildTicket();
                _mtsSdk.SendTicket(ticketCancel);
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
    }
}
