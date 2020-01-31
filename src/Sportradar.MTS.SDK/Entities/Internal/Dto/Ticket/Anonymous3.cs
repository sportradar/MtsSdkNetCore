/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Dto.Ticket
{
    /// <summary>
    /// Class for Ticket SelectionRef
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    internal partial class Anonymous3
    {
        public Anonymous3()
        { }

        public Anonymous3(int selectionIndex, bool banker)
        {
            _selectionIndex = selectionIndex;
            _banker = banker;
        }

        public Anonymous3(ISelectionRef selectionRef)
        {
            _selectionIndex = selectionRef.SelectionIndex;
            _banker = selectionRef.Banker;
        }
    }
}