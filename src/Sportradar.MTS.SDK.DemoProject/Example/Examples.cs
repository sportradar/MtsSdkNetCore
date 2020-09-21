/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sportradar.MTS.SDK.API;
using Sportradar.MTS.SDK.DemoProject.Tickets;
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Enums;
using Sportradar.MTS.SDK.Entities.EventArguments;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.DemoProject.Example
{
    /// <summary>
    /// Demonstrating ticket examples for creating and sending ticket
    /// </summary>
    public class Examples
    {
        private IMtsSdk _mtsSdk;

        private IBuilderFactory _factory;

        private readonly ILogger _log;
        private readonly ILoggerFactory _loggerFactory;

        public Examples(ILoggerFactory loggerFactory = null)
        {
            _loggerFactory = loggerFactory;
            _log = _loggerFactory?.CreateLogger(typeof(Basic)) ?? new NullLogger<Basic>();
        }

        public void Run()
        {
            _log.LogInformation("Running the MTS SDK ticket integration examples");

            _log.LogInformation("Retrieving configuration from application configuration file");
            var config = MtsSdk.GetConfiguration();

            _log.LogInformation("Creating root MTS SDK instance");
            _mtsSdk = new MtsSdk(config, _loggerFactory);

            _log.LogInformation("Attaching to events");
            AttachToFeedEvents(_mtsSdk);

            _log.LogInformation("Opening the sdk instance (creating and opening connection to the AMPQ broker)");
            _mtsSdk.Open();

            var ticketExamples = new TicketExamples(_mtsSdk.BuilderFactory);
            _factory = _mtsSdk.BuilderFactory;

            _log.LogInformation("Example 1");
            _mtsSdk.SendTicket(ticketExamples.Example1());
            _log.LogInformation("Example 2");
            _mtsSdk.SendTicket(ticketExamples.Example2());
            _log.LogInformation("Example 3");
            _mtsSdk.SendTicket(ticketExamples.Example3());
            _log.LogInformation("Example 4");
            _mtsSdk.SendTicket(ticketExamples.Example4());
            _log.LogInformation("Example 5");
            _mtsSdk.SendTicket(ticketExamples.Example5());
            _log.LogInformation("Example 6");
            _mtsSdk.SendTicket(ticketExamples.Example6());
            _log.LogInformation("Example 7");
            _mtsSdk.SendTicket(ticketExamples.Example7());
            _log.LogInformation("Example 8");
            _mtsSdk.SendTicket(ticketExamples.Example8());
            _log.LogInformation("Example 9");
            _mtsSdk.SendTicket(ticketExamples.Example9());
            _log.LogInformation("Example 10");
            _mtsSdk.SendTicket(ticketExamples.Example10());
            _log.LogInformation("Example 11");
            _mtsSdk.SendTicket(ticketExamples.Example11());
            _log.LogInformation("Example 12");
            _mtsSdk.SendTicket(ticketExamples.Example12());
            _log.LogInformation("Example 13");
            _mtsSdk.SendTicket(ticketExamples.Example13());
            _log.LogInformation("Example 14");
            _mtsSdk.SendTicket(ticketExamples.Example14());

            _log.LogInformation("Examples successfully executed. Hit <enter> to quit");
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
        }

        private void OnUnparsableTicketResponseReceived(object sender, UnparsableMessageEventArgs e)
        {
            _log.LogInformation($"Received unparsable ticket response: {e.Body}.");
        }

        private void OnSendTicketFailed(object sender, TicketSendFailedEventArgs e)
        {
            _log.LogInformation($"Sending ticket '{e.TicketId}' failed.");
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
                //mandatory for all cancellations, except for TimeOutTriggered cancellation
                ticket.Acknowledge();
            }
        }
    }
}
