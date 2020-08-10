// File Name:     HealingSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Monday, July 6, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class HealingSystem
    {
        /// <summary>
        /// Handles a player healing there team mates if they are wearing the nurse smiley.
        /// </summary>
        /// <param name="healer">The player that is healing.</param>
        /// <param name="playerToHeal">The player the healer is trying to heal.</param>
        public static void Handle(Player healer, Player playerToHeal)
        {
            if (healer.SmileyId != (int)Smiley.Nurse) // Only players with the Nurse smiley can heal team mates
            {
                return;
            }

            if (!healer.IsEnemiesWith(playerToHeal) && healer.IsNearPlayer(playerToHeal) && playerToHeal.Health < 100)
            {
                if (playerToHeal.Heal())
                {
                    CaptureTheFlagBot.SendPrivateMessage(playerToHeal, $"You were healed player {healer.Username}");
                    CaptureTheFlagBot.SendPrivateMessage(healer, $"You fully healed player {playerToHeal.Username}");
                }
            }
        }
    }
}