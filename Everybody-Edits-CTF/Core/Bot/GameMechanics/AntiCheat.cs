// File Name:     AntiCheat.cs
// By:            Darian Benam (GitHub: https://github.com/BeardedFish/)
// Date:          Friday, July 3, 2020

using Everybody_Edits_CTF.Core.Bot.DataStructures;
using Everybody_Edits_CTF.Helpers;

namespace Everybody_Edits_CTF.Core.Bot.GameMechanics
{
    public static class AntiCheat
    {
        /// <summary>
        /// Handles a player if they cheat during a Capture the Flag game.
        /// </summary>
        /// <param name="player">The player to be handled if they cheat.</param>
        public static void Handle(Player player)
        {
            // Remove flag from player if they have the enemy flag and enter God mode
            if (player.IsPlayingGame && player.IsInGodMode && player.HasEnemyFlag)
            {
                CaptureTheFlagBot.SendChatMessage($"ANTI-CHEAT! Player {player.Username} has used God mode while carrying the {TeamHelper.EnumToString(TeamHelper.GetOppositeTeam(player.Team))} teams flag!");

                CaptureTheFlag.Flags[TeamHelper.GetOppositeTeam(player.Team)].Return(null, false);
            }
        }
    }
}