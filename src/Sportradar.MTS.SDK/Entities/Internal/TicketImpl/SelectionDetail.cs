﻿/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    internal class SelectionDetail : ISelectionDetail
    {
        public int SelectionIndex { get; }
        public IResponseReason Reason { get; }
        public IRejectionInfo RejectionInfo { get; }

        public SelectionDetail(int selectionIndex, IResponseReason reason, IRejectionInfo rejectionInfo)
        {
            Guard.Argument(selectionIndex, nameof(selectionIndex)).InRange(0, 62);
            Guard.Argument(reason, nameof(reason)).NotNull();

            SelectionIndex = selectionIndex;
            Reason = reason;
            RejectionInfo = rejectionInfo;
        }
    }
}