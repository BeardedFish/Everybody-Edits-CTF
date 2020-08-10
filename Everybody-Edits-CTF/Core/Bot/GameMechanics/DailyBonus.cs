// File Name:     DailyBonus.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, August 10, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Database;
using System;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class DailyBonus
    {
        /// <summary>
        /// The amount of coins a player is awarded when they join the world on a new day.
        /// </summary>
        private const int CoinsAwardAmount = 500;

        /// <summary>
        /// Handles the daily bonus system for the Capture The Flag game. Daily bonus is awarded to players when their current login date is greater than their
        /// last login date. If that condition is not true, then this method does nothing.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(Player player)
        {
            PlayerDatabaseRow playerData = PlayersDatabaseTable.GetPlayerDatabaseRow(player.Username);

            if (playerData != null)
            {
                if (playerData.LastVisitDate > DateTime.Today)
                {
                    playerData.Coins += CoinsAwardAmount;
                    playerData.LastVisitDate = DateTime.Today;

                    CaptureTheFlagBot.SendPrivateMessage(player, $"Daily bonus! You have been awarded {CoinsAwardAmount} coins for revisiting the world.");
                }
            }
        }
    }
}