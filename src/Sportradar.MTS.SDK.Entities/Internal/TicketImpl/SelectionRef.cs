/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Dawn;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.TicketImpl
{
    public class SelectionRef : ISelectionRef
    {
        public int SelectionIndex { get; }
        public bool Banker { get; }

        public SelectionRef(int selectionIndex, bool isBanker)
        {
            Guard.Argument(selectionIndex).InRange(0, 62);

            SelectionIndex = selectionIndex;
            Banker = isBanker;
        }
    }
}