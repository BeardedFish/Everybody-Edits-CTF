﻿// File Name:     FightSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class FightSystem : IGameMechanic
    {
        public void Handle(string messageType, Player player)
        {
            if (!player.IsPlayingGame || player.IsInGodMode)
            {
                return;
            }

            foreach (Player enemyPlayer in JoinedWorld.Players.Values)
            {
                if (player == enemyPlayer)
                {
                    continue;
                }

                if (player.IsEnemiesWith(enemyPlayer) && player.IsNearPlayer(enemyPlayer))
                {
                    enemyPlayer.Attack(player);

                    HandleHealthStatusWarning(enemyPlayer);

                    if (enemyPlayer.Health <= 0)
                    {
                        enemyPlayer.Die();
                    }
                }
            }
        }

        /// <summary>
        /// Sends a player a private message if their health is at a certain level. The private message is to warn them that they are close to being killed by an enemy player.
        /// </summary>
        /// <param name="player">The player to warn about their health.</param>
        private void HandleHealthStatusWarning(Player player)
        {
            string healthDescription = null;

            switch (player.Health)
            {
                case 35:
                    healthDescription = "low";
                    break;
                case 10:
                    healthDescription = "critically low";
                    break;
            }

            if (healthDescription != null)
            {
                string msg = $"Warning! Your health is {healthDescription} ({player.Health} HP).";

                CtfBot.SendPrivateMessage(player, msg);
            }
        }
    }
}