﻿// File Name:     AttackSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, June 30, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Enums;
using Everybody_Edits_CTF.Helpers;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class AttackSystem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attacker"></param>
        public static void Handle(Player attacker, Player otherPlayer)
        {
            if (TeamHelper.IsEnemyPlayer(attacker.Team, otherPlayer.Team) && attacker.IsNearPlayer(otherPlayer))
            {
                otherPlayer.Attack(attacker);

                HandleHealthStatusWarning(otherPlayer);

                if (otherPlayer.Health <= 0)
                {
                    CaptureTheFlagBot.KillPlayer(otherPlayer, attacker, DeathReason.Player);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player">The player being attacked.</param>
        private static void HandleHealthStatusWarning(Player player)
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

                CaptureTheFlagBot.SendPrivateMessage(player, msg);
            }
        }
    }
}