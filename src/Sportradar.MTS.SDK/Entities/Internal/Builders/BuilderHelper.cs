using Sportradar.MTS.SDK.Entities.Builders;
using Sportradar.MTS.SDK.Entities.Interfaces;

namespace Sportradar.MTS.SDK.Entities.Internal.Builders
{
    internal static class BuilderHelper
    {
        public static IBet BuildBetFromExisting(IBetBuilder betBuilder, IBet existingBet, long newStake)
        {
            if (existingBet.Stake.Type.HasValue)
            {
                betBuilder.SetStake(newStake, existingBet.Stake.Type.Value);
            }
            else
            {
                betBuilder.SetStake(newStake);
            }
            if (existingBet.SumOfWins > 0)
            {
                betBuilder.SetSumOfWins(existingBet.SumOfWins);
            }
            if (existingBet.Bonus != null)
            {
                betBuilder.SetBetBonus(existingBet.Bonus.Value, existingBet.Bonus.Mode, existingBet.Bonus.Type, existingBet.Bonus.Description, existingBet.Bonus.PaidAs);
            }
            if (existingBet.FreeStake != null)
            {
                betBuilder.SetFreeStake(existingBet.FreeStake.Value, existingBet.FreeStake.Type, existingBet.FreeStake.Description, existingBet.FreeStake.PaidAs);
            }
            foreach (var ticketBetSelection in existingBet.Selections)
            {
                betBuilder.AddSelection(ticketBetSelection);
            }
            foreach (var ticketBetSelectedSystem in existingBet.SelectedSystems)
            {
                betBuilder.AddSelectedSystem(ticketBetSelectedSystem);
            }

            return betBuilder.Build();
        }
    }
}
