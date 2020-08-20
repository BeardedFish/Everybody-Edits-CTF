// File Name:     DailyBonus.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
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
        /// Handles the daily bonus system for the Capture The Flag game. Every day a player revisits the world, they are awarded <see cref="CoinsAwardAmount"/> coins.        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (PlayersTable.Loaded && PlayersTable.PlayerExists(player.Username))
            {
                PlayersTableRow playerData = PlayersTable.GetRow(player.Username);

                if (playerData != null && DateTime.Today > playerData.LastVisitDate)
                {
                    playerData.Statistics.Coins += CoinsAwardAmount;
                    playerData.LastVisitDate = DateTime.Today;

                    Task.Run(async() =>
                    {
                        await Task.Delay(PrivateMessageDelayMs);

                        ctfBot.SendPrivateMessage(player, $"Daily bonus! You have been awarded {CoinsAwardAmount} coins for revisiting the world.");
                    });
                }
            }
        }
    }
}