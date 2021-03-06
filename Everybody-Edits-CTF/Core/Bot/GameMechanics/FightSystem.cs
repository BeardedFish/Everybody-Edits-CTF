﻿// File Name:     AutoBalance.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class FightSystem
    {
        /// <summary>
        /// Game mechanic which handles a players fighting in the Capture The Flag game. A player can attack another player if they are not on the same team and if they
        /// spam WASD/arrow keys near the enemy player.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public FightSystem(CaptureTheFlagBot ctfBot)
        {
            ctfBot.OnPlayerMoved += OnPlayerMoved;
        }

        /// <summary>
        /// Handles players fighting in the Capture The Flag game.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnPlayerMoved(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!player.IsPlayingGame || player.IsInGodMode)
            {
                return;
            }

            foreach (Player enemyPlayer in ctfBot.JoinedWorld.Players.Values)
            {
                if (player == enemyPlayer)
                {
                    continue;
                }

                if (player.IsEnemiesWith(enemyPlayer) && player.IsNearPlayer(enemyPlayer))
                {
                    enemyPlayer.Attack(player);

                    HandleHealthStatusWarning(ctfBot, enemyPlayer);

                    if (enemyPlayer.Health <= 0)
                    {
                        enemyPlayer.Die(ctfBot);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a player a private message if their health is at a certain level. The private message is to warn them that they are close to being killed by an enemy player.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="player">The player to warn about their health.</param>
        private void HandleHealthStatusWarning(CaptureTheFlagBot ctfBot, Player player)
        {
            string healthDescription = string.Empty;

            switch (player.Health)
            {
                case 35:
                    healthDescription = "low";
                    break;
                case 10:
                    healthDescription = "critically low";
                    break;
            }

            if (healthDescription != string.Empty)
            {
                string msg = $"Warning! Your health is {healthDescription} ({player.Health} HP).";

                ctfBot.SendPrivateMessage(player, msg);
            }
        }
    }
}