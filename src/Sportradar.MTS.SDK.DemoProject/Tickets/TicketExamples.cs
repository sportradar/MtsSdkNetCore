/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System;
using System.Net;

namespace Sportradar.MTS.SDK.DemoProject.Tickets
{
    /// <summary>
    /// Examples how to build tickets from the MTS Ticket Integration guide (V31), chapter 10
    /// </summary>
    public class TicketExamples
    {
        private readonly IBuilderFactory _builderFactory;

        public TicketExamples(IBuilderFactory builderFactory)
        {
            _builderFactory = builderFactory;
        }

        private ISender GetSender()
        {
            return _builderFactory.CreateSenderBuilder()
                //.SetBookmakerId(1)
                //.SetLimitId(1)
                .SetSenderChannel(SenderChannel.Internet)
                .SetCurrency("EUR")
                .SetEndCustomer(IPAddress.Parse("1.2.3.4"), "Customer-" + DateTime.Now.Second, "EN", "deviceId-123", 10000)
                .Build();
        }

        /// <summary>
        /// 10.1 Ticket with Live single bet with 3-Way market
        /// </summary>
        /// <remarks>Ticket with single bet on Live soccer event (Event ID: 11057047), 3way, Away team (TypeID: 2; SubTypeID: 0; Special Odds value: *; Selection: 2)</remarks>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example1()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example1-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11057047")
                        .SetIdLo(2, 0, string.Empty, "2")
                        .SetOdds(12100)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.2 Pre-match Single bet Ticket
        /// </summary>
        /// <remarks>Ticket with single bet on Pre-match Soccer(Sport ID: 1) event (Event ID: 11050343), Halftime - 3way, Draw(oddstype: 42, Special Odds value: *; Selection: X)</remarks>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example2()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example2-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050343")
                        .SetIdLcoo(42, 1, string.Empty, "X")
                        .SetOdds(28700)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.3 Double Pre-match
        /// </summary>
        /// <remarks>Ticket with double bet on two pre-match selections: 
        /// Soccer (Sport ID: 1), Event ID: 11050343, Halftime - 3way (oddstype: 42), Special Odds value: *; Draw   
        /// Soccer (Sport ID: 1), Event ID: 10784408, Asian handicap first half (oddstype: 53), Special Odds value: 0.25; Home</remarks>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example3()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example3-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(2)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050343")
                        .SetIdLcoo(42, 1, string.Empty, "X")
                        .SetOdds(28700)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("10784408")
                        .SetIdLcoo(53, 1, "0.25", "1")
                        .SetOdds(16600)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.4 Treble on Pre-match and Live
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example4()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example4-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(3)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050343")
                        .SetIdLcoo(42, 1, string.Empty, "1")
                        .SetOdds(39600)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("10784408")
                        .SetIdLcoo(42, 1, string.Empty, "1")
                        .SetOdds(36600)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052925")
                        .SetIdLo(8, 518, "1-3", "NO")
                        .SetOdds(13799)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.5 System 3 / 4
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example5()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example5-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(3)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050343")
                        .SetIdLcoo(42, 1, string.Empty, "1")
                        .SetOdds(28700)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("10784408")
                        .SetIdLcoo(53, 1, "-0.25", "2")
                        .SetOdds(14800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11046885")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(11299)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "1")
                        .SetOdds(23500)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.6 SYSTEM 3 / 4 including 1 Banker
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example6()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example6-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(3)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050343")
                        .SetIdLcoo(42, 1, string.Empty, "1")
                        .SetOdds(28700)
                        .SetBanker(true)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("10784408")
                        .SetIdLcoo(53, 1, "-0.25", "2")
                        .SetOdds(14800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11046885")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(11299)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "1")
                        .SetOdds(23500)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.7 System 3 / 5 incl 1 Ways
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example7()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example7-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(3)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050343")
                        .SetIdLcoo(42, 1, string.Empty, "X")
                        .SetOdds(28700)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("10784408")
                        .SetIdLcoo(53, 1, "-0.25", "2")
                        .SetOdds(14800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11046885")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(11299)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "1")
                        .SetOdds(23500)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.8 System 3 / 5 including 1 Ways including 1 banker
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example8()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example8-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(3)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050343")
                        .SetIdLcoo(42, 1, string.Empty, "X")
                        .SetOdds(28700)
                        .SetBanker(true)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("10784408")
                        .SetIdLcoo(53, 1, "-0.25", "2")
                        .SetOdds(14800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11046885")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(11299)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "1")
                        .SetOdds(23500)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.9 Championship Outright
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example9()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example9-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("40777")
                        .SetIdLcoo(30, 14, string.Empty, "6495408")
                        .SetOdds(12200)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.10 Podium Outright
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example10()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example10-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("40987")
                        .SetIdLcoo(50, 14, string.Empty, "7080578")
                        .SetOdds(121600)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.11 Two single bets on the same event within one ticket
        /// </summary>
        /// <remarks>Ticket example where punter/better/endCustomer choose two selections from different markets, but from the same event</remarks>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example11()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example11-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B1-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050841")
                        .SetIdLcoo(10, 1, string.Empty, "1")
                        .SetOdds(17700)
                        .Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B2-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050841")
                        .SetIdLcoo(51, 1, "-1.25", "2")
                        .SetOdds(15600)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.12 4-Fold Accumulator with 80 Cents Sport-betting bonus
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example12()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example12-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B-" + DateTime.Now.Ticks)
                    .SetStake(10000, StakeType.Total)
                    .AddSelectedSystem(4)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050915")
                        .SetIdLcoo(20, 5, string.Empty, "1")
                        .SetOdds(14100)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(13600)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052893")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(16900)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11051699")
                        .SetIdLcoo(20, 5, string.Empty, "1")
                        .SetOdds(10900)
                        .Build())
                    .SetBetBonus(8000, BetBonusMode.All, BetBonusType.Total)
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.13 Multi-system bets ticket with different stakes
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example13()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example13-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B1-" + DateTime.Now.Ticks)
                    .SetStake(40000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050915")
                        .SetIdLcoo(20, 5, string.Empty, "1")
                        .SetOdds(14100)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052893")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(16900)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052537")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(10400)
                        .Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B2-" + DateTime.Now.Ticks)
                    .SetStake(60000, StakeType.Total)
                    .AddSelectedSystem(2)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050915")
                        .SetIdLcoo(20, 5, string.Empty, "1")
                        .SetOdds(14100)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052893")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(16900)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052537")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(10400)
                        .Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B3-" + DateTime.Now.Ticks)
                    .SetStake(120000, StakeType.Total)
                    .AddSelectedSystem(3)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050915")
                        .SetIdLcoo(20, 5, string.Empty, "1")
                        .SetOdds(14100)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052893")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(16900)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052537")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(10400)
                        .Build())
                    .Build())
                .BuildTicket();
        }

        /// <summary>
        /// 10.14 Multi-systems ticket with different number of selections and with different unit-stakes
        /// </summary>
        /// <returns>A <see cref="ITicket"/></returns>
        public ITicket Example14()
        {
            return _builderFactory.CreateTicketBuilder()
                .SetTicketId("Example14-" + DateTime.Now.Ticks)
                .SetSender(GetSender())
                .SetOddsChange(OddsChangeType.Any)
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B1-" + DateTime.Now.Ticks)
                    .SetStake(60000, StakeType.Total)
                    .AddSelectedSystem(3)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11050915")
                        .SetIdLcoo(20, 5, string.Empty, "1")
                        .SetOdds(14100)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052893")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(16900)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052531")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(15600)
                        .Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B2-" + DateTime.Now.Ticks)
                    .SetStake(120000, StakeType.Total)
                    .AddSelectedSystem(2)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052893")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(16900)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052531")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(15600)
                        .Build())
                    .Build())
                .AddBet(_builderFactory.CreateBetBuilder()
                    .SetBetId("B3-" + DateTime.Now.Ticks)
                    .SetStake(80000, StakeType.Total)
                    .AddSelectedSystem(1)
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11029671")
                        .SetIdLcoo(339, 5, "1.5", "2")
                        .SetOdds(16800)
                        .Build())
                    .AddSelection(_builderFactory.CreateSelectionBuilder()
                        .SetEventId("11052531")
                        .SetIdLcoo(20, 5, string.Empty, "2")
                        .SetOdds(15600)
                        .Build())
                    .Build())
                .BuildTicket();
        }
    }
}
