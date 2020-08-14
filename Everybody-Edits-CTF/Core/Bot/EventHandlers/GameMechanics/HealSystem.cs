// File Name:     HealSystem.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Thursday, August 13, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.EventHandlers.GameMechanics
{
    public sealed class HealSystem : IGameMechanic
    {
        /// <summary>
        /// Handles the heal system in the Capture The Flag game. The heal system only works for players who wear the <see cref="Smiley.Nurse"/>. A healer can only
        /// heal their team mates and they must spam WASD/arrow keys near their team mate in order to heal them.
        /// </summary>
        /// <param name="ctfBot">The Capture The Flag bot instance.</param>
        /// <param name="messageType">The <see cref="PlayerIOClient.Message.Type"/> that is calling this method.</param>
        /// <param name="player">The player to be handled.</param>
        public void Handle(CtfBot ctfBot, string messageType, Player player)
        {
            if (!player.IsPlayingGame || player.IsInGodMode || player.SmileyId != (int)Smiley.Nurse)
            {
                return;
            }

            foreach (Player allyPlayer in JoinedWorld.Players.Values)
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
    }
}