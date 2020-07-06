﻿// File Name:     HealingSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, July 6, 2020

using Everybody_Edits_CTF.Core.Bot;
using Everybody_Edits_CTF.Core.DataStructures;
using Everybody_Edits_CTF.Enums;
using Everybody_Edits_CTF.Helpers;

namespace Everybody_Edits_CTF.Core.GameMechanics
{
    public static class HealingSystem
    {
        public static void Handle(Player healer, Player playerToHeal)
        {
            if (healer.SmileyId != (int)Smilies.Nurse) // Only players with the Nurse smiley can heal team mates
            {
                return;
            }

            if (!TeamHelper.IsEnemyPlayer(healer.Team, playerToHeal.Team) && healer.IsNearPlayer(playerToHeal) && playerToHeal.Health < 100)
            {
                if (playerToHeal.Heal()) // Health restored fully
                {
                    CaptureTheFlagBot.SendPrivateMessage(playerToHeal, $"You were healed player {healer.Username}");
                    CaptureTheFlagBot.SendPrivateMessage(healer, $"You fully healed player {playerToHeal.Username}");
                }
            }
        }
    }
}