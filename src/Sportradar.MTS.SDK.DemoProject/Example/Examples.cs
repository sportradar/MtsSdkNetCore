/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using log4net;
using Sportradar.MTS.SDK.DemoProject.Tickets;

namespace Sportradar.MTS.SDK.DemoProject.Example
{
    /// <summary>
    /// Demonstrating ticket examples for creating and sending ticket
    /// </summary>
    public class Examples
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// The MTS SDK instance
        /// </summary>
        private IMtsSdk _mtsSdk;

        private IBuilderFactory _factory;

        public Examples(ILog log)
        {
            _log = log;
        }

        public void Run()
        {
            _log.Info("Running the MTS SDK ticket integration examples");

            _log.Info("Retrieving configuration from application configuration file");
            var config = MtsSdk.GetConfiguration();

            _log.Info("Creating root MTS SDK instance");
            _mtsSdk = new MtsSdk(config);

            _log.Info("Attaching to events");
            AttachToFeedEvents(_mtsSdk);

            _log.Info("Opening the sdk instance (creating and opening connection to the AMPQ broker)");
            _mtsSdk.Open();

            var ticketExamples = new TicketExamples(_mtsSdk.BuilderFactory);
            _factory = _mtsSdk.BuilderFactory;

            _log.Info("Example 1");
            _mtsSdk.SendTicket(ticketExamples.Example1());
            _log.Info("Example 2");
            _mtsSdk.SendTicket(ticketExamples.Example2());
            _log.Info("Example 3");
            _mtsSdk.SendTicket(ticketExamples.Example3());
            _log.Info("Example 4");
            _mtsSdk.SendTicket(ticketExamples.Example4());
            _log.Info("Example 5");
            _mtsSdk.SendTicket(ticketExamples.Example5());
            _log.Info("Example 6");
            _mtsSdk.SendTicket(ticketExamples.Example6());
            _log.Info("Example 7");
            _mtsSdk.SendTicket(ticketExamples.Example7());
            _log.Info("Example 8");
            _mtsSdk.SendTicket(ticketExamples.Example8());
            _log.Info("Example 9");
            _mtsSdk.SendTicket(ticketExamples.Example9());
            _log.Info("Example 10");
            _mtsSdk.SendTicket(ticketExamples.Example10());
            _log.Info("Example 11");
            _mtsSdk.SendTicket(ticketExamples.Example11());
            _log.Info("Example 12");
            _mtsSdk.SendTicket(ticketExamples.Example12());
            _log.Info("Example 13");
            _mtsSdk.SendTicket(ticketExamples.Example13());
            _log.Info("Example 14");
            _mtsSdk.SendTicket(ticketExamples.Example14());

            _log.Info("Examples successfully executed. Hit <enter> to quit");
            Console.WriteLine(string.Empty);
            Console.ReadLine();

            _log.Info("Detaching from events");
            DetachFromFeedEvents(_mtsSdk);

            _log.Info("Closing the connection and disposing the instance");
            _mtsSdk.Close();

            _log.Info("Example stopped");
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

            _log.Info("Attaching to events");
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

            _log.Info("Detaching from events");
            mtsSdk.SendTicketFailed -= OnSendTicketFailed;
            mtsSdk.TicketResponseReceived -= OnTicketResponseReceived;
            mtsSdk.UnparsableTicketResponseReceived -= OnUnparsableTicketResponseReceived;
        }

        private void OnTicketResponseReceived(object sender, TicketResponseReceivedEventArgs e)
        {
            _log.Info($"Received {e.Type}Response for ticket '{e.Response.TicketId}'.");

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
            _log.Info($"Received unparsable ticket response: {e.Body}.");
        }

        private void OnSendTicketFailed(object sender, TicketSendFailedEventArgs e)
        {
            _log.Info($"Sending ticket '{e.TicketId}' failed.");
        }

        private void HandleTicketResponse(ITicketResponse ticket)
        {
            _log.Info($"Ticket '{ticket.TicketId}' response is {ticket.Status}. Reason={ticket.Reason?.Message}");
            if (ticket.BetDetails != null && ticket.BetDetails.Any())
            {
                foreach (var betDetail in ticket.BetDetails)
                {
                    _log.Info($"Bet decline reason: '{betDetail.Reason?.Message}'.");
                    if (betDetail.SelectionDetails != null && betDetail.SelectionDetails.Any())
                    {
                        foreach (var selectionDetail in betDetail.SelectionDetails)
                        {
                            _log.Info($"Selection decline reason: '{selectionDetail.Reason?.Message}'.");
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
            _log.Info($"Ticket '{ticket.TicketId}' response is {ticket.Status}. Reason={ticket.Reason?.Message}");
            if (ticket.Status == TicketCancelAcceptance.Cancelled)
            {
                //required only if 'explicit acking' is enabled in MTS admin
                ticket.Acknowledge();
            }
        }
    }
}
