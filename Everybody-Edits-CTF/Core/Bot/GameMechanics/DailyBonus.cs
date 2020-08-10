// File Name:     DailyBonus.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, August 10, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Database;
using System;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class DailyBonus
    {
        /// <summary>
        /// The amount of coins a player is awarded when they join the world on a new day.
        /// </summary>
        private const int CoinsAwardAmount = 500;

        /// <summary>
        /// The delay the program should wait until sending a private message to the player that received a daily bonus.
        /// 
        /// NOTE: For some odd reason, if there is no delay the private message is not sent. Therefore, if the message is still not sending, increase this value.
        /// </summary>
        private const int PrivateMessageDelayMs = 1500;

        /// <summary>
        /// Handles the daily bonus system for the Capture The Flag game. Daily bonus is awarded to players when their current login date is greater than their
        /// last login date. If that condition is not true, then this method does nothing.
        /// </summary>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(Player player)
        {
            PlayerDatabaseRow playerData = PlayersDatabaseTable.GetRow(player.Username);

            if (playerData != null && DateTime.Today > playerData.LastVisitDate)
            {
                playerData.Coins += CoinsAwardAmount;
                playerData.LastVisitDate = DateTime.Today;

                Task.Run(async() =>
                {
                    await Task.Delay(PrivateMessageDelayMs);

                    CaptureTheFlagBot.SendPrivateMessage(player, $"Daily bonus! You have been awarded {CoinsAwardAmount} coins for revisiting the world.");
                });
            }
        }
    }
}