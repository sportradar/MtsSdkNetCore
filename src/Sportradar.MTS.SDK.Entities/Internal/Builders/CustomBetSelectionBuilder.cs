/*
 * Copyright (C) Sportradar AG. See LICENSE for full license governing this code
 */
using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces.CustomBet;
using Sportradar.MTS.SDK.Entities.Internal.CustomBetImpl;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    /// <summary>
    /// The run-time implementation of the <see cref="ICustomBetSelectionBuilder"/> interface
    /// </summary>
    public class CustomBetSelectionBuilder : ICustomBetSelectionBuilder
    {
        private string _eventId;
        private int _marketId;
        private string _specifiers;
        private string _outcomeId;

        public ICustomBetSelectionBuilder SetEventId(string eventId)
        {
            _eventId = eventId;
            return this;
        }

        public ICustomBetSelectionBuilder SetMarketId(int marketId)
        {
            _marketId = marketId;
            return this;
        }

        public ICustomBetSelectionBuilder SetSpecifiers(string specifiers)
        {
            _specifiers = specifiers;
            return this;
        }

        public ICustomBetSelectionBuilder SetOutcomeId(string outcomeId)
        {
            _outcomeId = outcomeId;
            return this;
        }

        public ISelection Build()
        {
            var selection = new Selection(_eventId, _marketId, _specifiers, _outcomeId);
            _eventId = null;
            _marketId = 0;
            _specifiers = null;
            _outcomeId = null;
            return selection;
        }

        public ISelection Build(string eventId, int marketId, string specifiers, string outcomeId)
        {
            _eventId = eventId;
            _marketId = marketId;
            _specifiers = specifiers;
            _outcomeId = outcomeId;
            return Build();
        }
    }
}
