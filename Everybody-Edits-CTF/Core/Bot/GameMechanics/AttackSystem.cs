// File Name:     AttackSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Tuesday, June 30, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class AttackSystem
    {
        /// <summary>
        /// Handles a player attacking an enemy player. If the attackee's health is less than or equal to zero, then the atackee is killed.
        /// </summary>
        /// <param name="attacker">The player attacking.</param>
        /// <param name="atackee">The player getting attacked.</param>
        public static void Handle(Player attacker, Player attackee)
        {
            if (attacker.IsEnemiesWith(attackee) && attacker.IsNearPlayer(attackee))
            {
                attackee.Attack(attacker);

                HandleHealthStatusWarning(attackee);

                if (attackee.Health <= 0)
                {
                    attackee.Die();
                }
            }
        }

        /// <summary>
        /// Sends a player a private message if their health is at a certain level. The private message is to warn them that they are close to being killed by an enemy player.
        /// </summary>
        /// <param name="player">The player to warn about their health.</param>
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