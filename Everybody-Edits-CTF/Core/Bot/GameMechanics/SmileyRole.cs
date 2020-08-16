// File Name:     SmileyRole.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;
using Everybody_Edits_CTF.Core.Bot.Enums;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class SmileyRole
    {
        /// <summary>
        /// Handles special smiley roles by sending a private message to a player if they wear a special smiley. A special smiley is able to perform a unique trait. For
        /// example, the nurse smiley can heal team mates.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        public static void Handle(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!player.IsPlayingGame)
            {
                return;
            }

            if (player.SmileyId == (int)Smiley.Nurse)
            {
                ctfBot.SendPrivateMessage(player, "You are now a healer for your team!");
            }
        }
    }
}