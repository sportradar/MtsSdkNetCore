/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using System.Collections.Generic;
using Dawn;
using System.Linq;
using Sportradar.MTS.SDK.Entities.Interfaces;
using Sportradar.MTS.SDK.Entities.Internal.Dto;
using Sportradar.MTS.SDK.Entities.Internal.Dto.TicketResponse;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    /// <summary>
    /// Implementation of <see cref="IBetDetail"/>
    /// </summary>
    /// <seealso cref="IBetDetail" />
    public class BetDetail : IBetDetail
    {
        /// <summary>
        /// Gets the id of the bet
        /// </summary>
        /// <value>The bet identifier</value>
        public string BetId { get; }
        /// <summary>
        /// Gets the bet response reason
        /// </summary>
        /// <value>The reason</value>
        public IResponseReason Reason { get; }
        /// <summary>
        /// Gets the bet reoffer details (mutually exclusive with <see cref="IAlternativeStake" />)
        /// </summary>
        /// <value>The reoffer</value>
        public IBetReoffer Reoffer { get; }
        /// <summary>
        /// Gets the alternative stake, mutually exclusive with <see cref="IBetReoffer" />
        /// </summary>
        /// <value>The alternative stake</value>
        public IAlternativeStake AlternativeStake { get; }
        /// <summary>
        /// Gets the array of selection details
        /// </summary>
        /// <value>The selection details</value>
        public IEnumerable<ISelectionDetail> SelectionDetails { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetDetail"/> class
        /// </summary>
        /// <param name="betId">The bet identifier</param>
        /// <param name="reason">The reason</param>
        /// <param name="alternativeStake">The alternative stake</param>
        /// <param name="reoffer">The reoffer</param>
        /// <param name="selectionDetails">The selection details</param>
        public BetDetail(string betId,
                         IResponseReason reason,
                         IAlternativeStake alternativeStake,
                         IBetReoffer reoffer,
                         IEnumerable<ISelectionDetail> selectionDetails)
        {
            Guard.Argument(betId).NotNull().NotEmpty().Require(betId.Length <= 128);
            Guard.Argument(reason).NotNull();

            BetId = betId;
            Reason = reason;
            AlternativeStake = alternativeStake;
            SelectionDetails = selectionDetails;
            Reoffer = reoffer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetDetail"/> class
        /// </summary>
        /// <param name="betDetail">The bet detail</param>
        public BetDetail(Anonymous2 betDetail)
        {
            Guard.Argument(betDetail).NotNull();

            BetId = betDetail.BetId;
            if (betDetail.Reason != null)
            {
                Reason = new ResponseReason(betDetail.Reason.Code,
                                            betDetail.Reason.Message);
            }
            if (betDetail.AlternativeStake != null && betDetail.AlternativeStake.Stake > 0)
            {
                AlternativeStake = new AlternativeStake(betDetail.AlternativeStake.Stake);
            }
            if (betDetail.SelectionDetails != null && betDetail.SelectionDetails.Any())
            {
                SelectionDetails = betDetail.SelectionDetails.ToList().ConvertAll(s =>
                                        new SelectionDetail(s.SelectionIndex,
                                                            new ResponseReason(s.Reason.Code, s.Reason.Message),
                                                            s.RejectionInfo == null ? null : new RejectionInfo(s.RejectionInfo.Id, s.RejectionInfo.EventId, s.RejectionInfo.Odds)));
            }
            if (betDetail.Reoffer != null && betDetail.Reoffer.Stake > 0)
            {
                Reoffer = new BetReoffer(betDetail.Reoffer.Stake, MtsTicketHelper.Convert(betDetail.Reoffer.Type));
            }
        }
    }
}