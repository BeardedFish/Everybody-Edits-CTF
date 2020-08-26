// File Name:     SmileyRoleNotification.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Saturday, August 15, 2020

using Everybody_Edits_CTF.Core.Bot.DataContainers;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public sealed class SmileyRoleNotification
    {
        /// <summary>
        /// Game mechanic which handles players changing their smiley to a special smiley. This class only notified players via a private message if they wear a special smiley.
        /// A special smiley is able to perform a unique trait in the Capture The Flag game. For example, the nurse smiley can heal teammates.
        /// </summary>
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        public SmileyRoleNotification(CaptureTheFlagBot ctfBot)
        {
            ctfBot.OnSmileyChanged += OnSmileyChanged;
        }

        /// <summary>
        /// Notifies a player via a private message if they wear a special smiley.
        /// <param name="ctfBot">The <see cref="CaptureTheFlagBot"/> instance.</param>
        /// <param name="player">The player to be handled.</param>
        private void OnSmileyChanged(CaptureTheFlagBot ctfBot, Player player)
        {
            if (!player.IsPlayingGame)
            {
                return;
            }

            if (HealSystem.CanHealTeammates(player))
            {
                ctfBot.SendPrivateMessage(player, "You are now a healer for your team!");
            }
            else if (DigSystem.WearingDiggableSmiley(player))
            {
                ctfBot.SendPrivateMessage(player, "You can now dig dirt in the world!");
            }
        }
    }
}