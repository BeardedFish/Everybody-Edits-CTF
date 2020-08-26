// File Name:     HealSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class HealSystem
    {
        /// <summary>
        /// Game mechanic which handles the heal system in the Capture The Flag game. The heal system only works for players who wears the <see cref="Smiley.Doctor"/> or
        /// the <see cref="Smiley.Nurse"/>. A healer can only heal their teammates and they must spam WASD/arrow keys near their teammate in order to heal them.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public HealSystem(CaptureTheFlagBot ctfBot)
        {
            ctfBot.OnPlayerMoved += OnPlayerMoved;
        }

        /// <summary>
        /// Handles a player trying to heal their teammate(s).
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnPlayerMoved(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!player.IsPlayingGame || player.IsInGodMode || !CanHealTeammates(player))
            {
                return;
            }

            foreach (Player allyPlayer in ctfBot.JoinedWorld.Players.Values)
            {
                if (player == allyPlayer)
                {
                    continue;
                }

                if (!player.IsEnemiesWith(allyPlayer) && player.IsNearPlayer(allyPlayer) && allyPlayer.Health < 100)
                {
                    if (allyPlayer.Heal())
                    {
                        ctfBot.SendPrivateMessage(allyPlayer, $"You were healed player {player.Username}");
                        ctfBot.SendPrivateMessage(player, $"You fully healed player {allyPlayer.Username}");
                    }
                }
            }
        }

        /// <summary>
        /// States whether a player can heal their teammates or not.
        /// 
        /// A player can heal their teammates if they are either wearing:
        ///     - Nurse smiley
        ///     - Doctor smiley
        /// </summary>
        /// <param name="player">The player to be checked.</param>
        /// <returns>True if the player can heal their teammates, if not, false.</returns>
        public static bool CanHealTeammates(Player player)
        {
            return player.SmileyId == (int)Smiley.Nurse || player.SmileyId == (int)Smiley.Doctor;
        }
    }
}