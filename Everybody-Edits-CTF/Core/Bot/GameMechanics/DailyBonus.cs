﻿// File Name:     DailyBonus.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Database;
using System;
using System.Threading.Tasks;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class DailyBonus
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
        /// Game mechanic which rewards players coins when they revisit the Everybody Edits world on a new day.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public DailyBonus(CaptureTheFlagBot ctfBot)
        {
            ctfBot.OnPlayerJoined += OnPlayerJoined;
        }

        /// <summary>
        /// Handles the daily bonus system. If the player is eligible to receive a daily bonus, they are rewarded N coins defined in the <see cref="CoinsAwardAmount"/>
        /// variable.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnPlayerJoined(CaptureTheFlagBot ctfBot, Player player)
        {
            if (MySqlDatabase.Loaded && MySqlDatabase.PlayerExists(player.Username))
            {
                PlayerData playerData = MySqlDatabase.GetRow(player.Username);

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