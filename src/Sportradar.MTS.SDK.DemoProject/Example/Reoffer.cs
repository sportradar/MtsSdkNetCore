/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Net;
using log4net;

namespace Sportradar.MTS.SDK.DemoProject.Example
{
    /// <summary>
    /// Example demonstrating how to build reoffer or cancel it
    /// </summary>
    public class Reoffer
    {
        /// <summary>
        /// The log
        /// </summary>
        private readonly ILog _log;

        private ITicket _originalTicket;

        /// <summary>
        /// The MTS SDK instance
        /// </summary>
        private IMtsSdk _mtsSdk;

        private IBuilderFactory _factory;

        public Reoffer(ILog log)
        {
            _log = log;
        }

        public void Run()
        {
            _log.Info("Running the MTS SDK reoffer example");

            _log.Info("Retrieving configuration from application configuration file");
            var config = MtsSdk.GetConfiguration();

            _log.Info("Creating root MTS SDK instance");
            _mtsSdk = new MtsSdk(config);

            _log.Info("Attaching to events");
            AttachToFeedEvents(_mtsSdk);

            _log.Info("Opening the sdk instance (creating and opening connection to the AMPQ broker)");
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
                            .SetId("lcoo:409/1/*/YES")
                            .SetOdds(11000)
                            .Build())
                        .Build())
                .BuildTicket();

            // send ticket to the MTS. Since this is a non-blocking way of sending, the response will raise the event TicketResponseReceived
            _log.Info("Send ticket to the MTS.");
            _mtsSdk.SendTicket(_originalTicket);

            _log.Info("Example successfully executed. Hit <enter> to quit");
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

        private void HandleTicketResponse(ITicketResponse ticketResponse)
        {
            _log.Info($"Ticket '{ticketResponse.TicketId}' response is {ticketResponse.Status}. Reason={ticketResponse.Reason?.Message}");
            if (ticketResponse.Status == TicketAcceptance.Accepted)
            {
                //required only if 'explicit acking' is enabled in MTS admin
                ticketResponse.Acknowledge();

                // handle ticket response

                //if for some reason we want to cancel ticket, this is how we can do it
                var ticketCancel = _factory.CreateTicketCancelBuilder().SetTicketId(ticketResponse.TicketId).SetCode(TicketCancellationReason.BookmakerTechnicalIssue).BuildTicket();
                _mtsSdk.SendTicket(ticketCancel);
            }
            else
            {
                // if the ticket was declined and response has reoffer, the reoffer or reoffer cancellation can be send
                // the reoffer or reoffer cancellation must be send before predefined timeout, or is automatically canceled
                if (ticketResponse.BetDetails.Any(a => a.Reoffer != null))
                {
                    if (ReofferShouldBeAccepted())
                    {
                        // ReSharper disable once RedundantArgumentDefaultValue
                        var reofferTicket = _factory.CreateTicketReofferBuilder().Set(_originalTicket, ticketResponse, null).BuildTicket();
                        _mtsSdk.SendTicket(reofferTicket);
                    }
                    else
                    {
                        var reofferCancel = _factory.CreateTicketReofferCancelBuilder().SetTicketId(ticketResponse.TicketId).BuildTicket();
                        _mtsSdk.SendTicket(reofferCancel);
                    }
                }
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

        private static bool ReofferShouldBeAccepted()
        {
            return new Random().Next(100) > 30;
        }
    }
}
